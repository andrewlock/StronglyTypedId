
        public static readonly TESTIDEfCoreValueConverter EfCoreValueConverter = new TESTIDEfCoreValueConverter();

        public class TESTIDEfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TESTID, System.Guid>
        {
            public TESTIDEfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    id => id.Value,
                    value => new TESTID(value),
                    mappingHints
                ) { }
        }