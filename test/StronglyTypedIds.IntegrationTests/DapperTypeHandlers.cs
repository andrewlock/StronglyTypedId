using System.Runtime.CompilerServices;
using Dapper;
using StronglyTypedIds.IntegrationTests.Types;

namespace StronglyTypedIds.IntegrationTests
{
    public static class DapperTypeHandlers
    {
        [ModuleInitializer]
        public static void AddHandlers()
        {
            SqlMapper.AddTypeHandler(new ConvertersGuidId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new ConvertersIntId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new ConvertersLongId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new ConvertersStringId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new ConvertersNullableStringId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new NewIdId1.DapperTypeHandler());
        }
    }
}