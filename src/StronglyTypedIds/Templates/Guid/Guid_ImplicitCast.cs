        public static implicit operator System.Guid(TESTID id) => id.Value;
        public static implicit operator TESTID(System.Guid value) => new(value);