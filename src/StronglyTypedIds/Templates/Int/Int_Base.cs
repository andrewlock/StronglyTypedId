    readonly partial struct TESTID : System.IComparable<TESTID>, System.IEquatable<TESTID>
    {
        public int Value { get; }

        public TESTID(int value)
        {
            Value = value;
        }

        public static readonly TESTID Empty = new TESTID(0);

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
