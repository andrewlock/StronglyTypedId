
        CONSTRUCTOR_VISIBILITY TESTID(string value)
        {
            Value = value ?? throw new System.ArgumentNullException(nameof(value));
        }
