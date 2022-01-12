#nullable enable
using System;

namespace SQLiteMapper.Exceptions
{
    public class DataEmptyException : Exception
    {
        public DataEmptyException(string? message) : base(message) { }
    }
}