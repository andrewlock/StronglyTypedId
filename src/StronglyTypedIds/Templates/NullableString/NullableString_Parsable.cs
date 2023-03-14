
        public static TESTID Parse(string s, IFormatProvider? provider)
        {
            return new TESTID(s);
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out TESTID result)
        {
            result = new TESTID(s);
            return true;
        }
