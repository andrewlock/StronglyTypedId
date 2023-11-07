
#if !FAKE_CODE
        public class AutoMapperTypeConverter : AutoMapper.ITypeConverter<TESTID, string?>
        {
            public string? Convert(TESTID source, string? destination, AutoMapper.ResolutionContext context)
            {
                return source.Value;
            }
        }
#endif
