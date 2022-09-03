        public static implicit operator int(TESTID id) => id.Value;
        public static implicit operator TESTID(int value) => new(value);