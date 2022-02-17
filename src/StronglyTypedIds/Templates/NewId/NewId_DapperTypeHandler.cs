
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TESTID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TESTID value)
            {
                parameter.Value = value.Value.ToGuid();
            }

            public override TESTID Parse(object value)
            {
                return value switch
                {
                    System.Guid guidValue => new TESTID(MassTransit.NewId.FromGuid(guidValue)),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && System.Guid.TryParse(stringValue, out var result) => new TESTID(MassTransit.NewId.FromGuid(result)),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }
        }