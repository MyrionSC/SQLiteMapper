using System;
using System.Linq;
using Newtonsoft.Json;
using SQLiteMapper.Exceptions;

namespace SQLiteMapper
{
    public static class ValidationHelper
    {
        public static void ValidateSchemaIfPresent(SqLiteMapperInput input)
        {
            if (input.schemas is null) return;

            // validate that schemas values are valid sqlite types
            var validTypeList = new[] { "TEXT", "INTEGER", "REAL", "BLOB" };
            foreach (var schema in input.schemas) {
                if (schema.Value is null) throw new ArgumentNullException(nameof(schema));
                foreach (var schemaKeyValue in schema.Value) {
                    if (!validTypeList.Contains(schemaKeyValue.Value)) {
                        throw new SchemaBadValueException(
                            $"declared value in schema keyvalue pair ({schemaKeyValue.Key}, {schemaKeyValue.Value}) is not valid sqlite type (which are: {JsonConvert.SerializeObject(validTypeList)})");
                    }
                }
            }

            // validate that schema keys correspond to data
            foreach (var key in input.schemas.Keys) {
                if (!input.data.ContainsKey(key)) {
                    throw new SchemaNoCorrespondingDataException(
                        $"schema key {key} does not have a corresponding data key");
                }
            }
        }
    }
}