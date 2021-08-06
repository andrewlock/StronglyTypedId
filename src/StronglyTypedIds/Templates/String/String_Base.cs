    readonly partial struct TESTID : System.IComparable<TESTID>, System.IEquatable<TESTID>
    {
        public string Value { get; }

        public TESTID(string value)
        {
            Value = value;
        }

        public static readonly TESTID Empty = new TESTID(string.Empty);

        public bool Equals(TESTID other) => this.Value.Equals(other.Value);
        public int CompareTo(TESTID other) => Value.CompareTo(other.Value);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TESTID other && Equals(other);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
        public static bool operator ==(TESTID a, TESTID b) => a.CompareTo(b) == 0;
        public static bool operator !=(TESTID a, TESTID b) => !(a == b);
