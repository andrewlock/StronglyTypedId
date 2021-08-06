
        class TESTIDTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is int intValue)
                {
                    return new TESTID(intValue);
                }

                return base.ConvertFrom(context, culture, value);
            }
        }