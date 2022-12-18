
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
                    byte byteValue => new TESTID(byteValue),
                    short shortValue when shortValue < byte.MaxValue => new TESTID((byte)shortValue),
                    int intValue when intValue < byte.MaxValue => new TESTID((byte)intValue),
                    long longValue when longValue < byte.MaxValue => new TESTID((byte)longValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && byte.TryParse(stringValue, out var result) => new TESTID(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }
        }