using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace SQLiteMapper
{
    public record SqLiteMapperInput
    {
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> data { get; set; }
        public string query { get; set; }
    }
}