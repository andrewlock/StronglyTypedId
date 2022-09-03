        public static explicit operator System.Guid(TESTID id) => id.Value;
        public static explicit operator TESTID(System.Guid value) => new(value);