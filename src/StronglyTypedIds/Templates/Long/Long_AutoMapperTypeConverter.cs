
#if !FAKE_CODE
        public class AutoMapperTypeConverter : AutoMapper.ITypeConverter<TESTID, long>
        {
            public long Convert(TESTID source, long destination, AutoMapper.ResolutionContext context)
            {
                return source.Value;
            }
        }
#endif
