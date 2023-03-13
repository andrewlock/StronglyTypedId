
        public static TESTID Parse(string s, IFormatProvider? provider)
        {
            return new TESTID(long.Parse(s));
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out TESTID result)
        {
            long res = 0;
            var ok = long.TryParse(out res);
            result = new TESTID(res);
            return ok;
        }
