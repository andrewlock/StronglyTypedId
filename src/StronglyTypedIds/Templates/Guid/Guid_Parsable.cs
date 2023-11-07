
#nullable enable
        public static TESTID Parse(string s, System.IFormatProvider? provider)
        {
            return new TESTID(Guid.Parse(s, provider));
        }

        public static bool TryParse(string? s, System.IFormatProvider? provider, out TESTID result)
        {
            Guid res = Guid.Empty;
            var ok = Guid.TryParse(s, provider, out res);
            result = new TESTID(res);
            return ok;
        }
#nullable disable
