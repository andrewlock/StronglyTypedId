
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TESTID, string>
        {
            public EfCoreValueConverter() : this(null) { }
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    id => id.Value == MongoDB.Bson.ObjectId.Empty ? null : id.Value.ToString(),
                    value => string.IsNullOrEmpty(value) ? TESTID.Empty : new TESTID(new MongoDB.Bson.ObjectId(value)),
                    mappingHints
                ) { }
        }