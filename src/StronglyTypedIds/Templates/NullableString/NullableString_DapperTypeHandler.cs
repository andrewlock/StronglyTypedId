
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TESTID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TESTID value)
            {
                parameter.Value = value.Value;
                parameter.DbType = System.Data.DbType.AnsiString;
            }

            public override TESTID Parse(object value)
            {
                return value switch
                {
                    null => new TESTID(null),
                    System.DBNull => new TESTID(null),
                    string stringValue => new TESTID(stringValue),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }
        }

#pragma warning disable CA2255
        [System.Runtime.CompilerServices.ModuleInitializerAttribute]
        public static void AddTypeHandler()
        {
            Dapper.SqlMapper.AddTypeHandler(new DapperTypeHandler());
        }
#pragma warning restore CA2255
