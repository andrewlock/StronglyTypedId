
        public static TESTID Parse(string s, IFormatProvider? provider)
        {
            return new TESTID(int.Parse(s));
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out TESTID result)
        {
            int res = 0;
            var ok = int.TryParse(out res);
            result = new TESTID(res);
            return ok;
        }
