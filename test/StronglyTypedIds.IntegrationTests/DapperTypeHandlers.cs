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
            SqlMapper.AddTypeHandler(new DapperGuidId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new DapperIntId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new DapperStringId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new DapperLongId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new DapperNullableStringId.DapperTypeHandler());
            SqlMapper.AddTypeHandler(new DapperNewIdId.DapperTypeHandler());
        }
    }
}