using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IntegrationMaker.Model;
using Swashbuckle.Swagger.Annotations;
using IntegrationMaker.Service.InegrationServices;
using IntegrationMaker.Service.InegrationServices.Concrete;
using System.Configuration;
using System.Security.Claims;

namespace IntegrationMaker.Controllers
{
    
    public class IntegrationController : ApiController
    {
        // OWIN auth middleware constants
        public const string scopeElement = "http://schemas.microsoft.com/identity/claims/scope";
        public const string objectIdElement = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        // API Scopes
        public static string ReadPermission = ConfigurationManager.AppSettings["api:ReadScope"];
        public static string WritePermission = ConfigurationManager.AppSettings["api:WriteScope"];
        // Validate to ensure the necessary scopes are present.
        private void HasRequiredScopes(String permission)
        {
            if (!ClaimsPrincipal.Current.FindFirst(scopeElement).Value.Contains(permission))
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ReasonPhrase = $"The Scope claim does not contain the {permission} permission."
                });
            }
        }
        // GET api/values
        [SwaggerOperation("GetAll")]
        public HttpResponseMessage Get()
        {
            HasRequiredScopes(ReadPermission);
            string owner = ClaimsPrincipal.Current.FindFirst(objectIdElement).Value;
            var service = new GetIntegrationService();
           var integrations = service.GetIntegrations();
           return Request.CreateResponse<List<IntegrationModel>>(System.Net.HttpStatusCode.Accepted, integrations);
        }

        // POST api/values
        [SwaggerOperation("CreateIntegration")]
        [SwaggerResponse(HttpStatusCode.Created,"Created. Integration is created and the connections is validated")]
        [SwaggerResponse(HttpStatusCode.PreconditionFailed, "PreconditionFailed. Did not provied profecient parameter model")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "InternalServererror. UPS something whent wrong")]

        [SwaggerResponse(HttpStatusCode.BadRequest, "BadRequest. Intergration could not be validated")]
        public HttpResponseMessage Post(IntegrationModel integrationModel)
        {
            try
            {
                try
                {
                    var validateIntegrationModelservice = new ValidateIntegrationModelService();
                    validateIntegrationModelservice.ValidateIntegrationModel(integrationModel);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse<IntegrationModel>(System.Net.HttpStatusCode.PreconditionFailed, integrationModel);
                }
                var createIntegrationService = new CreateIntegrationService();
                var createdIntegrationModel = createIntegrationService.CreateIntegrationAsync(integrationModel).Result;
                return Request.CreateResponse<IntegrationModel>(System.Net.HttpStatusCode.Created, createdIntegrationModel);
            }
            catch(Exception ex)
            {
               return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
