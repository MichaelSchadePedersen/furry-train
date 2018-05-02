using IntegrationMaker.Entities;
using IntegrationMaker.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationMaker.Service.InegrationServices.Concrete
{
   public class GetIntegrationTypeService
    {
        public GetIntegrationTypeService()
        { }
        public List<IntegrationTypeModel> GetIntegrationTypes()
        {
            var integrationTypes = SelectQuery(Data.Get<IntegrationType>().AsQueryable());
            return integrationTypes.ToList();
        }

        private IQueryable<IntegrationTypeModel> SelectQuery(IQueryable<IntegrationType> organizations)
        {
            return organizations.Select(x => new IntegrationTypeModel
            {
                Name = x.Name
            });
        }
    }
}
