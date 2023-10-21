
#nullable enable
        public static TESTID Parse(string s, System.IFormatProvider? provider)
        {
            return new TESTID(int.Parse(s, provider));
        }

        public static bool TryParse(string? s, System.IFormatProvider? provider, out TESTID result)
        {
            int res = 0;
            var ok = int.TryParse(s, provider, out res);
            result = new TESTID(res);
            return ok;
        }
#nullable disable
