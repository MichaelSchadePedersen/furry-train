using IntegrationMaker.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationMaker.Service.InegrationServices.Concrete
{
    public class ValidateIntegrationModelService
    {
        public ValidateIntegrationModelService()
        { }

        public void ValidateIntegrationModel(IntegrationModel integrationModel)
        {
            var IsValidIntergrationModel = true;
            IsValidIntergrationModel = (integrationModel is null || integrationModel.IntegrationCredentialModel is null || integrationModel.IntegrationTypeModel is null) ? false : true;
            if (!IsValidIntergrationModel)
                throw new Exception();
        }
    }
}
