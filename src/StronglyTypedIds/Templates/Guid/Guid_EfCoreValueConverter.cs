
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TESTID, System.Guid>
        {
            public EfCoreValueConverter() : this(null) { }
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    id => id.Value,
                    value => new TESTID(value),
                    mappingHints
                ) { }
        }
        
        public class EfCoreValueGenerator : Microsoft.EntityFrameworkCore.ValueGeneration.ValueGenerator<TESTID>
        { 
            public override bool GeneratesTemporaryValues => false;

            public override TESTID Next(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
            { 
                return TESTID.New();
            }
        }