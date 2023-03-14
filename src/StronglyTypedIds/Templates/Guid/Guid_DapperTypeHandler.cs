
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TESTID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TESTID value)
            {
                parameter.Value = value.Value;
                parameter.DbType = System.Data.DbType.Guid;
            }

            public override TESTID Parse(object value)
            {
                return value switch
                {
                    System.Guid guidValue => new TESTID(guidValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && System.Guid.TryParse(stringValue, out var result) => new TESTID(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }

            [System.Runtime.CompilerServices.ModuleInitializerAttribute]
            public static void AddTypeHandler()
            {
                Dapper.SqlMapper.AddTypeHandler(new DapperTypeHandler());
            }
#pragma warning restore CA2255

        }
