
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TESTID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TESTID value)
            {
                parameter.Value = value.Value.ToSequentialGuid();
            }

            public override TESTID Parse(object value)
            {
                return value switch
                {
                    System.Guid guidValue => new TESTID(MassTransit.NewId.FromSequentialGuid(guidValue)),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && System.Guid.TryParse(stringValue, out var result) => new TESTID(MassTransit.NewId.FromSequentialGuid(result)),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }
        }