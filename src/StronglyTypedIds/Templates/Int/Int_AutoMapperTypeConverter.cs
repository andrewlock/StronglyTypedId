
#if !FAKE_CODE
        public class AutoMapperTypeConverter : AutoMapper.ITypeConverter<TESTID, int>
        {
            public int Convert(TESTID source, int destination, AutoMapper.ResolutionContext context)
            {
                return source.Value;
            }
        }
#endif
