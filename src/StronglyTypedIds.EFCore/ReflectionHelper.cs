using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StronglyTypedIds.EFCore
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type>? GetAllInheritedTypes<T>() where T : class
        {
            return Assembly.GetAssembly(typeof(T))
                    ?.GetTypes()
                    .Where(t => t.IsClass &&
                    !t.IsAbstract &&
                    typeof(T).IsAssignableFrom(t));
        }

        public static IEnumerable<Type>? GetAllInheritedTypesForGeneric(Type type)
        {
            return Assembly.GetAssembly(type)
                    ?.GetTypes()
                    .Where(t => t.IsClass &&
                    !t.IsAbstract &&
                    IsAssignableToGenericType(t, type));
        }

        public static IEnumerable<Type>? GetAllTypesEndsWith(string endsWith)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass
                            && t.IsSealed
                            && t.IsAbstract
                            && t.Name.EndsWith(endsWith));
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            if (baseType is null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
