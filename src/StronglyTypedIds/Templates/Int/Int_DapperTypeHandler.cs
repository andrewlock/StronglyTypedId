﻿
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<PLACEHOLDERID>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, PLACEHOLDERID value)
            {
                parameter.Value = value.Value;
            }

            public override PLACEHOLDERID Parse(object value)
            {
                return value switch
                {
                    int intValue => new PLACEHOLDERID(intValue),
                    long longValue when longValue < int.MaxValue => new PLACEHOLDERID((int)longValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && int.TryParse(stringValue, out var result) => new PLACEHOLDERID(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to PLACEHOLDERID"),
                };
            }
        }