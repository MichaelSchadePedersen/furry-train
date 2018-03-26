using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IntegrationMaker.Models;
using Swashbuckle.Swagger.Annotations;
using IntegrationMaker.Service.InegrationServices;
using IntegrationMaker.Service.InegrationServices.Concrete;
using IntegrationMaker.Service.InegrationServices.Models;

namespace IntegrationMaker.Controllers
{
    public class IntegrationController : ApiController
    {

        // GET api/values
        [SwaggerOperation("GetAll")]
        public HttpResponseMessage Get()
        {
           var service = new GetIntegrationService();
           var integrations = service.GetIntegrations();
           return Request.CreateResponse<List<IntegrationModel>>(System.Net.HttpStatusCode.Accepted, integrations);
        }

        // POST api/values
        [SwaggerOperation("CreateIntegration")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public HttpResponseMessage Post(IntegrationModel integrationModel)
        {
            var createIntegrationService = new CreateIntegrationService();
            var createdIntegrationModel = createIntegrationService.CreateIntegration(integrationModel);
            var response = Request.CreateResponse<IntegrationModel>(System.Net.HttpStatusCode.Created, createdIntegrationModel);
            return response;
        }
    }
}
