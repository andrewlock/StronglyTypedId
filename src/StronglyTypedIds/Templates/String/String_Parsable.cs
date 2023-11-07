
#nullable enable
        public static TESTID Parse(string s, System.IFormatProvider? provider)
        {
            return new TESTID(s.ToString(provider));
        }

        public static bool TryParse(string? s, System.IFormatProvider? provider, out TESTID result)
        {
            var ok = s != null;
            if (ok)
            {
                result = new TESTID(s!.ToString(provider));
            }
            else
            {
                result = new TESTID("");
            }
            return ok;
        }
#nullable disable
