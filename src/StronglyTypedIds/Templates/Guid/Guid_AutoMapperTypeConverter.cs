
#if !FAKE_CODE
        public class AutoMapperTypeConverter : AutoMapper.ITypeConverter<TESTID, System.Guid>
        {
            public System.Guid Convert(TESTID source, System.Guid destination, AutoMapper.ResolutionContext context)
            {
                return source.Value;
            }
        }
#endif
