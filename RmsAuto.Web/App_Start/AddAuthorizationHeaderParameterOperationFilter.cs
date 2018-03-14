using Swashbuckle.Swagger;
using System.Web.Http.Description;

namespace RMSAutoAPI
{
    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.operationId.Equals("Auth_Auth"))
                return;
            if (operation.parameters != null)
            {
                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @in = "header",
                    description = "Bearer access-token",
                    required = false,
                    type = "string"
                });
            }
        }
    }
}