
using IntegrationMaker.BusinessLogic.Brokers.ERPBroker;
using IntegrationMaker.Entities;
using IntegrationMaker.Model;

using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationMaker.Service.InegrationServices.Concrete
{
   public class CreateIntegrationService
   {
        public CreateIntegrationService()
        { }
        public async System.Threading.Tasks.Task<IntegrationModel> CreateIntegrationAsync(IntegrationModel integrationModel)
        {
            var broker = ErpBroker.GetInstance(integrationModel, integrationModel.IntegrationCredentialModel);
            await broker.VerifyIntegration();
            return integrationModel;
        }
    }
}
