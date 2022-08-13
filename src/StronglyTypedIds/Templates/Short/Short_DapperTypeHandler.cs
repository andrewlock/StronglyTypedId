
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
                    short shortValue => new TESTID(shortValue),
                    byte byteValue => new TESTID(byteValue),
                    int intValue when intValue < byte.MaxValue => new TESTID((short)intValue),
                    long longValue when longValue < byte.MaxValue => new TESTID((short)longValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && short.TryParse(stringValue, out var result) => new TESTID(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }
        }