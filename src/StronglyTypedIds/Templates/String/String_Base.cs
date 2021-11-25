    readonly partial struct TESTID : INTERFACES
    {
        public string Value { get; }

        public TESTID(string value)
        {
            Value = value ?? throw new System.ArgumentNullException(nameof(value));
        }

        public static readonly TESTID Empty = new TESTID(string.Empty);

        public bool Equals(TESTID other)
        {
            return (Value, other.Value) switch
            {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                (_, _) => Value.Equals(other.Value),
            };
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TESTID other && Equals(other);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
        public static bool operator ==(TESTID a, TESTID b) => a.Equals(b);
        public static bool operator !=(TESTID a, TESTID b) => !(a == b);
