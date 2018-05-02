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

namespace IntegrationMaker.Controllers
{
    public class CompanyController : ApiController
    {

        
        
        public HttpResponseMessage Post(LeadModel leadModel)
        {
            try
            {
                try
                {
                    var insertRecordService = new InsertRecordService();
                    insertRecordService.CreateLeadAsync(leadModel);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse<LeadModel>(System.Net.HttpStatusCode.PreconditionFailed, leadModel);
                }
               
                return Request.CreateResponse<LeadModel>(System.Net.HttpStatusCode.Created, leadModel);
            }
            catch(Exception ex)
            {
               return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        
        


    }
}
