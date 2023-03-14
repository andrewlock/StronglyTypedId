
        public static TESTID Parse(string s, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out TESTID result)
        {
            result = new TESTID(s);
            return true;
        }
