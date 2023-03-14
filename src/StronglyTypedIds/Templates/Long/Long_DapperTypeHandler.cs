
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TESTID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TESTID value)
            {
                parameter.Value = value.Value;
                parameter.DbType = System.Data.DbType.Int64;
            }

            public override TESTID Parse(object value)
            {
                return value switch
                {
                    long longValue => new TESTID(longValue),
                    int intValue => new TESTID(intValue),
                    decimal decimalValue => new TESTID((long)decimalValue),
                    short shortValue => new TESTID(shortValue),
                    string stringValue when  !string.IsNullOrEmpty(stringValue) && long.TryParse(stringValue, out var result) => new TESTID(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }

#pragma warning disable CA2255
            [System.Runtime.CompilerServices.ModuleInitializerAttribute]
            public static void AddTypeHandler()
            {
                Dapper.SqlMapper.AddTypeHandler(new DapperTypeHandler());
            }
#pragma warning restore CA2255
        }
