        public static implicit operator MassTransit.NewId(TESTID id) => id.Value;
        public static implicit operator TESTID(MassTransit.NewId value) => new(value);