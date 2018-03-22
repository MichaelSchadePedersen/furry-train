using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;

namespace IntegrationMaker.Controllers
{
    public class IntegrationController : ApiController
    {
        
        // POST api/values
        [SwaggerOperation("CreateIntegration")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post()
        {
        }
        
    }
}
