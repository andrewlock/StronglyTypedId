
#if !FAKE_CODE
public static void UseLinqToDbConverter(LinqToDB.DataOptions opts)
{
    var mappingSchema = new LinqToDB.Mapping.MappingSchema();

    mappingSchema.SetDataType(typeof(TESTID), LinqToDB.DataType.VarChar);
    mappingSchema
        .SetConvertExpression<TESTID, LinqToDB.Data.DataParameter>(a => new LinqToDB.Data.DataParameter() { DataType = LinqToDB.DataType.VarChar, Value = a.Value});

    LinqToDB.DataOptionsExtensions.UseMappingSchema(opts, mappingSchema);
}

public static TESTID op_Implicit(string value)
{
    return new TESTID(value);
}

#endif
