﻿using System;

/// <summary>
/// JSON framework used to serialize/deserialize strongly-typed ID value
/// </summary>
[Flags]
public enum StronglyTypedIdJsonConverter
{
    NewtonsoftJson = 1,
    SystemTextJson = 2
}