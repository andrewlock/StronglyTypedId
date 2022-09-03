        public static explicit operator string(TESTID id) => id.Value;
        public static explicit operator TESTID(string value) => new(value);