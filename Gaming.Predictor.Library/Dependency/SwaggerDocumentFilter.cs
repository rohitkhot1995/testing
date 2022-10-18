using Gaming.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi.Models;

namespace Gaming.Predictor.Library.Dependency
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        protected readonly String _BasePath;
        public SwaggerDocumentFilter(IOptions<Application> appSettings)
        {
            _BasePath = appSettings.Value.CustomSwaggerConfig.BasePath;
        }

        //public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext documentFilterContext)
        //{
        //    swaggerDoc.BasePath = _BasePath;
        //    //swaggerDoc.Host = "some-url-that-is-hosted-on-azure.azurewebsites.net";
        //    //swaggerDoc.Schemes = new List<string> { "https" };
        //}

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext documentFilterContext)
        {
            //swaggerDoc.BasePath = _BasePath;
            swaggerDoc.Servers.Add(new OpenApiServer() { Url = _BasePath });

            //swaggerDoc.Host1 = "some-url-that-is-hosted-on-azure.azurewebsites.net";
            //swaggerDoc.Schemes = new List<string> { "https" };
        }
    }
}
