﻿
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TESTID, long>
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
            private long _id = long.MinValue;
            public override bool GeneratesTemporaryValues => true;

            public override TESTID Next(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
            {
                _id += 1;
                return new TESTID(_id);
            }
        }