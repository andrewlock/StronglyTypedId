    readonly partial struct TESTID : INTERFACES
    {
        public System.Guid Value { get; }

        public TESTID(System.Guid value)
        {
            Value = value;
        }

        public static TESTID New() => new TESTID(System.Guid.NewGuid());
        public static readonly TESTID Empty = new TESTID(System.Guid.Empty);

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
