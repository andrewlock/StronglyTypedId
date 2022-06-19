using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StronglyTypedIds.EFCore;

public static class Extensions
{
    public static void RegisterStrongTypedIdDynamically(
        this ModelBuilder modelBuilder,
        IMutableEntityType entityData,
        DbContext context,
        string idPropertyName = "Id"
        )
    {
        var entityMethod = typeof(ModelBuilder).GetMethods()
            .First(m => m.IsGenericMethod && m.Name == (nameof(ModelBuilder.Entity)))
            !.MakeGenericMethod(entityData.ClrType);
        var propertyMethod = typeof(EntityTypeBuilder).GetMethods()
            .First(m => !m.IsGenericMethod && m.Name == nameof(EntityTypeBuilder.Property));
        var hasConversionMethod = typeof(PropertyBuilder).GetMethods()
            .First(m => !m.IsGenericMethod && m.Name == nameof(PropertyBuilder.HasConversion));

        var entityTypeBuilder = entityMethod.Invoke(modelBuilder, Array.Empty<object>()) as EntityTypeBuilder;
        var propertyBuilder = propertyMethod!.Invoke(entityTypeBuilder, new[] { idPropertyName }) as PropertyBuilder;

        var idProperty = entityData.GetProperty(idPropertyName);
        var idType = idProperty.ClrType;
        var converterType = idType.GetNestedType("EfCoreValueConverter");

        if (converterType is null)
        {
            return;
        }

        hasConversionMethod!.Invoke(propertyBuilder, new[] { converterType });

        var realIdType = idType.GetProperty("Value")?.PropertyType;

        if (new[] { typeof(int), typeof(long), typeof(short) }.Contains(realIdType))
        {
            Type? propertyBuilderExtensionType = null;
            var propertyBuilderExtensionTypes = ReflectionHelper.GetAllTypesEndsWith("PropertyBuilderExtensions");

            if (propertyBuilderExtensionTypes is null || !propertyBuilderExtensionTypes.Any())
            {
                return;
            }

            if (propertyBuilderExtensionTypes?.Count() == 1)
            {
                propertyBuilderExtensionType = propertyBuilderExtensionTypes.First();
            }
            else
            {
                propertyBuilderExtensionType = propertyBuilderExtensionTypes!.First(x => x.AssemblyQualifiedName!.Contains(context.Database.ProviderName!));
            }

            if (propertyBuilderExtensionType is null)
            {
                return;
            }

            var useIdentityColumnMethod = propertyBuilderExtensionType.GetMethods()
                .OrderBy(m => m.GetParameters().Length)
                .First(m => !m.IsGenericMethod && m.Name == "UseIdentityColumn");

            var useIdentityColumnParameters = new List<object>() { propertyBuilder! };
            switch (context.Database.ProviderName)
            {
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    break;

                case "Microsoft.EntityFrameworkCore.SqlServer":
                    useIdentityColumnParameters.Add(1);
                    useIdentityColumnParameters.Add(1);
                    break;

                case "Pomelo.EntityFrameworkCore.MySql":
                case "MySql.EntityFrameworkCore":
                    break;
            }
            useIdentityColumnMethod!.Invoke(null, useIdentityColumnParameters.ToArray());
        }
    }
}