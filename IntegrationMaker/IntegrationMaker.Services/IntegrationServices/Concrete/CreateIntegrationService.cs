
using IntegrationMaker.BusinessLogic.Brokers.ERPBroker;
using IntegrationMaker.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationMaker.Service.InegrationServices.Concrete
{
   public class CreateIntegrationService
   {
        public CreateIntegrationService()
        { }
        public IntegrationModel CreateIntegration(IntegrationModel integrationModel)
        {
          var t =  ErpBroker.ImportType.Full;
            return integrationModel;
        }
    }
}
