using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Misc;

public class AddServersDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new() { Url = "https://localhost:5001", Description = "Local" },
            new() { Url = "https://voyagerapi.com.tr/api", Description = "Production" },
            new() { Url = "https://staging.voyagerapi.com.tr/api", Description = "Staging" }
        };
    }
}
