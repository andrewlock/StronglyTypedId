
        class TESTIDTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
            {
                if (value is null)
                {
                    return new TESTID(null);
                }

                var stringValue = value as string;
                if (stringValue is not null)
                {
                    return new TESTID(stringValue);
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Type? sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
            }

            public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType)
            {
                if (value is TESTID idValue)
                {
                    if (destinationType == typeof(string))
                    {
                        return idValue.Value;
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }