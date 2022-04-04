    readonly partial struct TESTID : INTERFACES
    {
        public MongoDB.Bson.ObjectId Value { get; }

        public TESTID(MongoDB.Bson.ObjectId value)
        {
            Value = value;
        }

        public static TESTID New() => new TESTID(MongoDB.Bson.ObjectId.GenerateNewId());
        public static readonly TESTID Empty = new TESTID(MongoDB.Bson.ObjectId.Empty);

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