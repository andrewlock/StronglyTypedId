        public static implicit operator long(TESTID id) => id.Value;
        public static implicit operator TESTID(long value) => new(value);