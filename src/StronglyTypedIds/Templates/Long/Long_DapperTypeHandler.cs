
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TESTID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TESTID value)
            {
                parameter.Value = value.Value;
            }

            public override TESTID Parse(object value)
            {
                return value switch
                {
                    long longValue => new TESTID(longValue),
                    int intValue => new TESTID(intValue),
                    short shortValue => new TESTID(shortValue),
                    string stringValue when  !string.IsNullOrEmpty(stringValue) && long.TryParse(stringValue, out var result) => new TESTID(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }
        }