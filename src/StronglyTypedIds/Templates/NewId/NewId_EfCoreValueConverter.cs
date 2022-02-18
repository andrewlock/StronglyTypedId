
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TESTID, System.Guid>
        {
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    id => id.Value.ToGuid(),
                    value => new TESTID(MassTransit.NewId.FromGuid(value)),
                    mappingHints
                ) { }
        }