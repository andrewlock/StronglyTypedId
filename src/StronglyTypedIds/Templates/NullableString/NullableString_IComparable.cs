#pragma warning disable CA1036
        public int CompareTo(TESTID other)
        {
            return (Value, other.Value) switch
            {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                (_, _) => Value.CompareTo(other.Value),
            };
        }
        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(obj, null))
                return 1;

            if (obj is not TESTID other)
                throw new System.ArgumentException("Object is not a TESTID");

            return Value.CompareTo(other.Value);
        }
#pragma warning restore CA1036
