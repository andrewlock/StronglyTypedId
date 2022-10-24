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

        public static TESTID Parse(string value) => new TESTID(System.Guid.Parse(value));
        public static bool TryParse(string value, out TESTID result)
        {
            if (System.Guid.TryParse(value, out System.Guid parseResult))
            {
                result = new TESTID(parseResult);
                return true;
            }
            result = default;
            return false;
        }
