
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<PLACEHOLDERID, int>
        {
            public EfCoreValueConverter() : this(null) { }
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    id => id.Value,
                    value => new PLACEHOLDERID(value),
                    mappingHints
                ) { }
        }