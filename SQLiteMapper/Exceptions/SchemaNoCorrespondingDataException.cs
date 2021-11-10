using System;

namespace SQLiteMapper.Exceptions
{
    public class SchemaNoCorrespondingDataException : Exception
    {
        public SchemaNoCorrespondingDataException(string? message) : base(message) { }
    }
}