
        class TESTIDTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(long) || sourceType == typeof(int) || sourceType == typeof(short)  || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return value switch
                {
                    long longValue => new TESTID(longValue),
                    int intValue => new TESTID(intValue),
                    short shortValue => new TESTID(shortValue),
                    string stringValue when  !string.IsNullOrEmpty(stringValue) && long.TryParse(stringValue, out var result) => new TESTID(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }

            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(long) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is TESTID idValue)
                {
                    if (destinationType == typeof(long))
                    {
                        return idValue.Value;
                    }

                    if (destinationType == typeof(string))
                    {
                        return idValue.Value.ToString();
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }