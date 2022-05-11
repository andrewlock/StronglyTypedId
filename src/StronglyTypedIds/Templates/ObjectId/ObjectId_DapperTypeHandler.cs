
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TESTID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TESTID value)
            {
                parameter.Value = value.Value == MongoDB.Bson.ObjectId.Empty ? null : value.Value.ToString();
            }

            public override TESTID Parse(object value)
            {
                return value switch
                {
                    string stringValue when !string.IsNullOrEmpty(stringValue) => new TESTID(new MongoDB.Bson.ObjectId(stringValue)),
                    string stringValue when string.IsNullOrEmpty(stringValue) => TESTID.Empty,
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }
        }