
#if !FAKE_CODE
public static void UseLinqToDbConverter(LinqToDB.Mapping.MappingSchema mappingSchema)
{
    mappingSchema.SetConvertExpression<TESTID, LinqToDB.Data.DataParameter>(a => new LinqToDB.Data.DataParameter() { DataType = LinqToDB.DataType.VarChar, Value = a.Value});
    mappingSchema.SetConvertExpression<LinqToDB.Data.DataParameter, string>(p => (string)p.Value);
}

public static TESTID op_Implicit(string value)
{
    return new TESTID(value);
}

#endif
