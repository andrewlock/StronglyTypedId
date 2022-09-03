        public static implicit operator string(TESTID id) => id.Value;
        public static implicit operator TESTID(string value) => new(value);