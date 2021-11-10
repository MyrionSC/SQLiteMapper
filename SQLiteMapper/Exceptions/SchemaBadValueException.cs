#nullable enable
using System;

namespace SQLiteMapper.Exceptions
{
    public class SchemaBadValueException : Exception
    {
        public SchemaBadValueException(string? message) : base(message) { }
    }
}