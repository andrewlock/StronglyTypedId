
#if !FAKE_CODE
public static void UseLinqToDbConverter(LinqToDB.Mapping.MappingSchema mappingSchema)
{
    mappingSchema.SetConvertExpression<TESTID, LinqToDB.Data.DataParameter>(a => new LinqToDB.Data.DataParameter() { DataType = LinqToDB.DataType.Decimal, Value = a.Value});
    mappingSchema.SetConvertExpression<LinqToDB.Data.DataParameter, long>(p => (long)p.Value);
}

public static TESTID op_Implicit(decimal value)
{
    return new TESTID((long)value);
}
#endif
