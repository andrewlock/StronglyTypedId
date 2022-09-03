        public static explicit operator long(TESTID id) => id.Value;
        public static explicit operator TESTID(long value) => new(value);