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