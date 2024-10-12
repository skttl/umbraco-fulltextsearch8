using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using NJsonSchema.Generation;

namespace Our.Umbraco.FullTextSearch.SchemaGenerator;

internal class SchemaGenerator
{
    private readonly JsonSchemaGenerator _schemaGenerator;

    public SchemaGenerator()
    {
        _schemaGenerator = new JsonSchemaGenerator(
            new FullTextSearchSchemaGeneratorSettings());
    }

    public string Generate() 
    {
        var schema = GenerateFullTextSearchSchema();
        return schema.ToString();
    }

    private JObject GenerateFullTextSearchSchema()
    {
        var schema = _schemaGenerator.Generate(typeof(AppSettings));
        return JsonConvert.DeserializeObject<JObject>(schema.ToJson());
    }
   
}

internal class FullTextSearchSchemaGeneratorSettings : JsonSchemaGeneratorSettings
{
    public FullTextSearchSchemaGeneratorSettings()
    {
        AlwaysAllowAdditionalObjectProperties = true;
        SerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new WritablePropertiesOnlyResolver(),
        };
        DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
        SchemaNameGenerator = new NamespacePrefixedSchemaNameGenerator();
        SerializerSettings.Converters.Add(new StringEnumConverter());
        IgnoreObsoleteProperties = true;
        GenerateExamples  = true;
    }

    private class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            var result = props.Where(p => p.Writable).ToList();
            result.ForEach(x => x.PropertyName = ToPascalCase(x.PropertyName));
            return result;
        }

        private string ToPascalCase(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return char.ToUpperInvariant(str[0]) + str.Substring(1);
            }

            return str;

        }
    }
}

internal class NamespacePrefixedSchemaNameGenerator : DefaultSchemaNameGenerator
{
    public override string Generate(Type type) => type.Namespace.Replace(".", string.Empty) + base.Generate(type);
}
