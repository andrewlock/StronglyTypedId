namespace StronglyTypedIds;

internal static partial class EmbeddedSources
{
    private const string LongTemplate = """
        partial struct PLACEHOLDERID :
    #if NET6_0_OR_GREATER
            global::System.ISpanFormattable,
    #endif
    #if NET7_0_OR_GREATER
            global::System.IParsable<PLACEHOLDERID>, global::System.ISpanParsable<PLACEHOLDERID>,
    #endif
    #if NET8_0_OR_GREATER
            global::System.IUtf8SpanParsable<PLACEHOLDERID>, global::System.IUtf8SpanFormattable,
    #endif
            global::System.IComparable<PLACEHOLDERID>, global::System.IEquatable<PLACEHOLDERID>, global::System.IFormattable
        {
            public long Value { get; }
    
            public PLACEHOLDERID(long value)
            {
                Value = value;
            }
    
            public static readonly PLACEHOLDERID Empty = new PLACEHOLDERID(0);
    
            /// <inheritdoc cref="global::System.IEquatable{T}"/>
            public bool Equals(PLACEHOLDERID other) => this.Value.Equals(other.Value);
            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is PLACEHOLDERID other && Equals(other);
            }
    
            public override int GetHashCode() => Value.GetHashCode();
    
            public override string ToString() => Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture);
    
            public static bool operator ==(PLACEHOLDERID a, PLACEHOLDERID b) => a.Equals(b);
            public static bool operator !=(PLACEHOLDERID a, PLACEHOLDERID b) => !(a == b);
            public static bool operator >  (PLACEHOLDERID a, PLACEHOLDERID b) => a.CompareTo(b) > 0;
            public static bool operator <  (PLACEHOLDERID a, PLACEHOLDERID b) => a.CompareTo(b) < 0;
            public static bool operator >=  (PLACEHOLDERID a, PLACEHOLDERID b) => a.CompareTo(b) >= 0;
            public static bool operator <=  (PLACEHOLDERID a, PLACEHOLDERID b) => a.CompareTo(b) <= 0;
    
            /// <inheritdoc cref="global::System.IComparable{TSelf}"/>
            public int CompareTo(PLACEHOLDERID other) => Value.CompareTo(other.Value);
    
            public partial class PLACEHOLDERIDTypeConverter : global::System.ComponentModel.TypeConverter
            {
                public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type sourceType)
                {
                    return sourceType == typeof(long) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
                }
            
                public override object? ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object value)
                {
                    return value switch
                    {
                        long longValue => new PLACEHOLDERID(longValue),
                        string stringValue when !string.IsNullOrEmpty(stringValue) && long.TryParse(stringValue, out var result) => new PLACEHOLDERID(result),
                        _ => base.ConvertFrom(context, culture, value),
                    };
                }
    
                public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type? sourceType)
                {
                    return sourceType == typeof(long) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
                }
    
                public override object? ConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object? value, global::System.Type destinationType)
                {
                    if (value is PLACEHOLDERID idValue)
                    {
                        if (destinationType == typeof(long))
                        {
                            return idValue.Value;
                        }
    
                        if (destinationType == typeof(string))
                        {
                            return idValue.Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
    
                    return base.ConvertTo(context, culture, value, destinationType);
                }
            }
    
            public partial class PLACEHOLDERIDSystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<PLACEHOLDERID>
            {
                public override bool CanConvert(global::System.Type typeToConvert)
                    => typeToConvert == typeof(long) || typeToConvert == typeof(string) || base.CanConvert(typeToConvert);
    
                public override PLACEHOLDERID Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
                    => new (reader.GetInt64());
    
                public override void Write(global::System.Text.Json.Utf8JsonWriter writer, PLACEHOLDERID value, global::System.Text.Json.JsonSerializerOptions options)
                    => writer.WriteNumberValue(value.Value);
    
    #if NET6_0_OR_GREATER
                public override PLACEHOLDERID ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
                    => new(long.Parse(reader.GetString() ?? throw new global::System.FormatException("The string for the PLACEHOLDERID property was null")));
    
                public override void WriteAsPropertyName(global::System.Text.Json.Utf8JsonWriter writer, PLACEHOLDERID value, global::System.Text.Json.JsonSerializerOptions options)
                    => writer.WritePropertyName(value.Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
    #endif
            }
    
            public static PLACEHOLDERID Parse(string input)
                => new(long.Parse(input));
    
    #if NET7_0_OR_GREATER
            /// <inheritdoc cref="global::System.IParsable{TSelf}"/>
            public static PLACEHOLDERID Parse(string input, global::System.IFormatProvider? provider)
                => new(long.Parse(input, provider));
    
            /// <inheritdoc cref="global::System.IParsable{TSelf}"/>
            public static bool TryParse(
                [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? input,
                global::System.IFormatProvider? provider,
                out PLACEHOLDERID result)
            {
                if (input is null)
                {
                    result = default;
                    return false;
                }
    
                if (long.TryParse(input, provider, out var value))
                {
                    result = new(value);
                    return true;
                }
    
                result = default;
                return false;
            }
    #endif
    
            /// <inheritdoc cref="global::System.IFormattable"/>
            public string ToString(
    #if NET7_0_OR_GREATER
                [global::System.Diagnostics.CodeAnalysis.StringSyntax(global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.NumericFormat)]
    #endif
                string? format,
                global::System.IFormatProvider? formatProvider)
                => Value.ToString(format, formatProvider);
    
    #if NETCOREAPP2_1_OR_GREATER
            public static PLACEHOLDERID Parse(global::System.ReadOnlySpan<char> input)
                => new(long.Parse(input));
    #endif
    
    #if NET6_0_OR_GREATER
    #if NET7_0_OR_GREATER
            /// <inheritdoc cref="global::System.ISpanParsable{TSelf}"/>
    #endif
            public static PLACEHOLDERID Parse(global::System.ReadOnlySpan<char> input, global::System.IFormatProvider? provider)
    #if NET7_0_OR_GREATER
                => new(long.Parse(input, provider));
    #else
                => new(long.Parse(input));
    #endif
            
    #if NET7_0_OR_GREATER
            /// <inheritdoc cref="global::System.ISpanParsable{TSelf}"/>
    #endif
            public static bool TryParse(global::System.ReadOnlySpan<char> input, global::System.IFormatProvider? provider, out PLACEHOLDERID result)
            {
    #if NET7_0_OR_GREATER
                if (long.TryParse(input, provider, out var value))
    #else
                if (long.TryParse(input, out var value))
    #endif
                {
                    result = new(value);
                    return true;
                }
    
                result = default;
                return false;
            }
    
            /// <inheritdoc cref="global::System.ISpanFormattable"/>
            public bool TryFormat(
                global::System.Span<char> destination,
                out int charsWritten,
    #if NET7_0_OR_GREATER
                [global::System.Diagnostics.CodeAnalysis.StringSyntax(global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.GuidFormat)]
    #endif
                global::System.ReadOnlySpan<char> format,
                global::System.IFormatProvider? provider)
                => Value.TryFormat(destination, out charsWritten, format);
    
            /// <inheritdoc cref="global::System.ISpanFormattable"/>
            public bool TryFormat(
                global::System.Span<char> destination,
                out int charsWritten,
    #if NET7_0_OR_GREATER
                [global::System.Diagnostics.CodeAnalysis.StringSyntax(global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.GuidFormat)]
    #endif
                global::System.ReadOnlySpan<char> format = default)
                => Value.TryFormat(destination, out charsWritten, format);
    #endif
    #if NET8_0_OR_GREATER
            /// <inheritdoc cref="global::System.IUtf8SpanFormattable.TryFormat" />
            public bool TryFormat(
                global::System.Span<byte> utf8Destination,
                out int bytesWritten,
                [global::System.Diagnostics.CodeAnalysis.StringSyntax(global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.NumericFormat)]
                global::System.ReadOnlySpan<char> format = default,
                global::System.IFormatProvider? provider = null)
                => Value.TryFormat(utf8Destination, out bytesWritten, format, provider);
    
            /// <inheritdoc cref="global::System.IUtf8SpanParsable{TSelf}.Parse(ReadOnlySpan{byte}, IFormatProvider?)" />
            public static PLACEHOLDERID Parse(global::System.ReadOnlySpan<byte> utf8Text, global::System.IFormatProvider? provider)
                => new(long.Parse(utf8Text, provider));
    
            /// <inheritdoc cref="global::System.IUtf8SpanParsable{TSelf}.TryParse(ReadOnlySpan{byte}, IFormatProvider?, out TSelf)" />
            public static bool TryParse(global::System.ReadOnlySpan<byte> utf8Text, global::System.IFormatProvider? provider, out PLACEHOLDERID result)
            {
                if (long.TryParse(utf8Text, provider, out var intResult))
                {
                    result = new PLACEHOLDERID(intResult);
                    return true;
                }
    
                result = default;
                return false;
            }
    #endif
        }
    """;
}