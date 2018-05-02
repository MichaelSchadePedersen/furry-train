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
using IntegrationMaker.Services.CRMServices.Concrete;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace IntegrationMaker.Controllers
{
    public class ZohoWebhookController : ApiController
    {

        
        
        public async Task<IHttpActionResult> ZohoSubscriptionCreated([FromBody]JObject body)
        {
            return Ok();
        }
        
        


    }
}
