using System.Linq;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Bouvet.Syndicate.TestProject.Configuration.StartupConfigs
{
    public class FileUploadDocumentProcessor : IDocumentProcessor
    {
        public void Process(DocumentProcessorContext context)
        {
            foreach (var (_, operation) in context.Document.Paths.SelectMany(x => x.Value))
            {
                for (var i = 0; i < operation.Parameters.Count; i++)
                {
                    var operationParameter = operation.Parameters[i];
                    if (operationParameter.Type == NJsonSchema.JsonObjectType.File)
                    {
                        operation.Parameters.RemoveAt(i);
                        i--;
                        if (operation.RequestBody == null)
                        {
                            operation.RequestBody = new NSwag.OpenApiRequestBody();
                            var content = new NSwag.OpenApiMediaType();
                            operation.RequestBody.Content.Add("multipart/form-data", content);
                            content.Schema = new NJsonSchema.JsonSchema()
                            {
                                Type = NJsonSchema.JsonObjectType.Object,
                            };
                        }
                        var multipartFormData = operation.RequestBody.Content["multipart/form-data"];

                        multipartFormData.Schema.Properties.Add(operationParameter.Name, new NJsonSchema.JsonSchemaProperty()
                        {
                            Type = NJsonSchema.JsonObjectType.String,
                            Format = "binary",
                        });
                    }
                }
            }
        }
    }
}
