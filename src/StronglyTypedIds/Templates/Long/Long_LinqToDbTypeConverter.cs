
#if !FAKE_CODE
public static void UseLinqToDbConverter(LinqToDB.DataOptions opts)
{
    var mappingSchema = new LinqToDB.Mapping.MappingSchema();

    mappingSchema.SetDataType(typeof(TESTID), LinqToDB.DataType.Decimal);
    mappingSchema
        .SetConvertExpression<TESTID, LinqToDB.Data.DataParameter>(a => new LinqToDB.Data.DataParameter() { DataType = LinqToDB.DataType.Decimal, Value = a.Value});

    LinqToDB.DataOptionsExtensions.UseMappingSchema(opts, mappingSchema);
}

public static TESTID op_Implicit(decimal value)
{
    return new TESTID((long)value);
}
#endif
