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

namespace IntegrationMaker.Controllers
{
    public class IntegrationTypeController : ApiController
    {

        // GET api/values
        [SwaggerOperation("GetAll")]
        public HttpResponseMessage Get()
        {
           var service = new GetIntegrationTypeService();
           var integrationTypes = service.GetIntegrationTypes();
           return Request.CreateResponse<List<IntegrationTypeModel>>(System.Net.HttpStatusCode.Accepted, integrationTypes);
        }
        
    }
}
