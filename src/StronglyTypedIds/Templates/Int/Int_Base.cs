    readonly partial struct TESTID : INTERFACES
    {
        public int Value { get; }

        public TESTID(int value)
        {
            Value = value;
        }

        public static readonly TESTID Empty = new TESTID(0);

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

        public static TESTID Parse(string value) => new TESTID(int.Parse(value));
        public static bool TryParse(string value, out TESTID result)
        {
            if (int.TryParse(value, out int parseResult))
            {
                result = new TESTID(parseResult);
                return true;
            }
            result = default;
            return false;
        }
