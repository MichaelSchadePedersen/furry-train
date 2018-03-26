using IntegrationMaker.Service.InegrationServices.Models;
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
            return new IntegrationModel();
        }
    }
}
