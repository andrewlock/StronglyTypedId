
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TESTID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TESTID value)
            {
                parameter.Value = value.Value.ToString();
            }

            public override TESTID Parse(object value)
            {
                return value switch
                {
                    string stringValue when !string.IsNullOrEmpty(stringValue) => new TESTID(new MongoDB.Bson.ObjectId(stringValue)),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TESTID"),
                };
            }
        }