
        class TESTIDTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(long) || sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return value switch
                {
                    long longValue => new TESTID(longValue),
                    int intValue => new TESTID(intValue),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }
        }