        public static explicit operator int(TESTID id) => id.Value;
        public static explicit operator TESTID(int value) => new(value);