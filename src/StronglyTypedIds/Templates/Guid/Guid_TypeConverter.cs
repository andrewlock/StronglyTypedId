
        class TESTIDTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                var stringValue = value as string;
                if (!string.IsNullOrEmpty(stringValue)
                    && System.Guid.TryParse(stringValue, out var guid))
                {
                    return new TESTID(guid);
                }

                return base.ConvertFrom(context, culture, value);
            }
        }