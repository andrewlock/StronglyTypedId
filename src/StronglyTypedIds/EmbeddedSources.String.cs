namespace StronglyTypedIds;

internal static partial class EmbeddedSources
{
    private const string StringTemplate = """
        partial struct PLACEHOLDERID :
    #if NET6_0_OR_GREATER
            global::System.ISpanFormattable,
    #endif
    #if NET7_0_OR_GREATER
            global::System.IParsable<PLACEHOLDERID>, global::System.ISpanParsable<PLACEHOLDERID>,
    #endif
            global::System.IComparable<PLACEHOLDERID>, global::System.IEquatable<PLACEHOLDERID>, global::System.IFormattable
        {
            public string Value { get; }
    
            public PLACEHOLDERID(string value)
            {
                Value = value ?? throw new global::System.ArgumentNullException(nameof(value));
            }
    
            public static readonly PLACEHOLDERID Empty = new PLACEHOLDERID(string.Empty);
    
            /// <inheritdoc cref="global::System.IEquatable{T}"/>
            public bool Equals(PLACEHOLDERID other)
                => (Value, other.Value) switch
                {
                    (null, null) => true,
                    (null, _) => false,
                    (_, null) => false,
                    (_, _) => Value.Equals(other.Value, global::System.StringComparison.Ordinal),
                };

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is PLACEHOLDERID other && Equals(other);
            }
    
            public override int GetHashCode() => Value.GetHashCode();
    
            public override string ToString() => Value;
    
            public static bool operator ==(PLACEHOLDERID a, PLACEHOLDERID b) => a.Equals(b);
            public static bool operator !=(PLACEHOLDERID a, PLACEHOLDERID b) => !(a == b);
            public static bool operator >  (PLACEHOLDERID a, PLACEHOLDERID b) => a.CompareTo(b) > 0;
            public static bool operator <  (PLACEHOLDERID a, PLACEHOLDERID b) => a.CompareTo(b) < 0;
            public static bool operator >=  (PLACEHOLDERID a, PLACEHOLDERID b) => a.CompareTo(b) >= 0;
            public static bool operator <=  (PLACEHOLDERID a, PLACEHOLDERID b) => a.CompareTo(b) <= 0;
    
            /// <inheritdoc cref="global::System.IComparable{TSelf}"/>
            public int CompareTo(PLACEHOLDERID other)
                => (Value, other.Value) switch
                {
                    (null, null) => 0,
                    (null, _) => -1,
                    (_, null) => 1,
                    (_, _) => string.CompareOrdinal(Value, other.Value),
                };
    
            public partial class PLACEHOLDERIDTypeConverter : global::System.ComponentModel.TypeConverter
            {
                public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type sourceType)
                {
                    return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
                }
            
                public override object? ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object value)
                {
                    if (value is string stringValue)
                    {
                        return new PLACEHOLDERID(stringValue);
                    }
                    
                    return base.ConvertFrom(context, culture, value);
                }
    
                public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type? sourceType)
                {
                    return sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
                }
    
                public override object? ConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object? value, global::System.Type destinationType)
                {
                    if (value is PLACEHOLDERID idValue)
                    {
                        if (destinationType == typeof(string))
                        {
                            return idValue.Value;
                        }
                    }
    
                    return base.ConvertTo(context, culture, value, destinationType);
                }
            }
    
            public partial class PLACEHOLDERIDSystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<PLACEHOLDERID>
            {
                public override PLACEHOLDERID Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
                    => new (reader.GetString()!);
    
                public override void Write(global::System.Text.Json.Utf8JsonWriter writer, PLACEHOLDERID value, global::System.Text.Json.JsonSerializerOptions options)
                    => writer.WriteStringValue(value.Value);
    
    #if NET6_0_OR_GREATER
                public override PLACEHOLDERID ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
                    => new(reader.GetString() ?? throw new global::System.FormatException("The string for the PLACEHOLDERID property was null"));
    
                public override void WriteAsPropertyName(global::System.Text.Json.Utf8JsonWriter writer, PLACEHOLDERID value, global::System.Text.Json.JsonSerializerOptions options)
                    => writer.WritePropertyName(value.Value);
    #endif
            }
    
            public static PLACEHOLDERID Parse(string input)
                => new(input);
    
    #if NET7_0_OR_GREATER
            /// <inheritdoc cref="global::System.IParsable{TSelf}"/>
            public static PLACEHOLDERID Parse(string input, global::System.IFormatProvider? provider)
                => new(input);
    
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
    
                result = new(input);
                return true;
            }
    #endif
    
            /// <inheritdoc cref="global::System.IFormattable"/>
            public string ToString(string? format, global::System.IFormatProvider? formatProvider)
                => Value;
    
    #if NETCOREAPP2_1_OR_GREATER
            public static PLACEHOLDERID Parse(global::System.ReadOnlySpan<char> input)
                => new(input.ToString());
    #endif
    
    #if NET6_0_OR_GREATER
    #if NET7_0_OR_GREATER
            /// <inheritdoc cref="global::System.ISpanParsable{TSelf}"/>
    #endif
            public static PLACEHOLDERID Parse(global::System.ReadOnlySpan<char> input, global::System.IFormatProvider? provider)
                => new(input.ToString());
            
    #if NET7_0_OR_GREATER
            /// <inheritdoc cref="global::System.ISpanParsable{TSelf}"/>
    #endif
            public static bool TryParse(global::System.ReadOnlySpan<char> input, global::System.IFormatProvider? provider, out PLACEHOLDERID result)
            {
                result = new(input.ToString());
                return true;
            }
    
            /// <inheritdoc cref="global::System.ISpanFormattable"/>
            public bool TryFormat(
                global::System.Span<char> destination,
                out int charsWritten,
                global::System.ReadOnlySpan<char> format,
                global::System.IFormatProvider? provider)
                => TryFormat(destination, out charsWritten, format);
    
            /// <inheritdoc cref="global::System.ISpanFormattable"/>
            public bool TryFormat(
                global::System.Span<char> destination,
                out int charsWritten,
                global::System.ReadOnlySpan<char> format = default)
            {
                if (destination.Length > Value.Length)
                {
                    global::System.MemoryExtensions.AsSpan(Value).CopyTo(destination);
                    charsWritten = Value.Length;
                    return true;
                }
            
                charsWritten = default;
                return false;
            }
    #endif
        }
    """;
}