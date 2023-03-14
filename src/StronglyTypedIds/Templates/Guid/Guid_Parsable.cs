
        public static TESTID Parse(string s, System.IFormatProvider? provider)
        {
            return new TESTID(Guid.Parse(s));
        }

        public static bool TryParse(string? s, System.IFormatProvider? provider, out TESTID result)
        {
            long res = 0;
            var ok = Guid.TryParse(s, out res);
            result = new TESTID(res);
            return ok;
        }
