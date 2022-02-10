    readonly partial struct TESTID : INTERFACES
    {
        public MassTransit.NewId Value { get; }

        public TESTID(MassTransit.NewId value)
        {
            Value = value;
        }

        public static TESTID New() => new TESTID(MassTransit.NewId.Next());
        public static readonly TESTID Empty = new TESTID(MassTransit.NewId.Empty);

        public bool Equals(TESTID other) => this.Value.Equals(other.Value);
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TESTID other && Equals(other);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
        public static bool operator ==(TESTID a, TESTID b) => a.Equals(b);
        public static bool operator !=(TESTID a, TESTID b) => !(a == b);
