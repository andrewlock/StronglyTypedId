        public static explicit operator MassTransit.NewId(TESTID id) => id.Value;
        public static explicit operator TESTID(MassTransit.NewId value) => new(value);