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
            // SqlMapper.AddTypeHandler(new DapperStringId.DapperTypeHandler());
            // SqlMapper.AddTypeHandler(new DapperLongId.DapperTypeHandler());
            // SqlMapper.AddTypeHandler(new DapperNullableStringId.DapperTypeHandler());
            // SqlMapper.AddTypeHandler(new DapperNewIdId.DapperTypeHandler());
        }
    }
}