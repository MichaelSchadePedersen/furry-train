using IntegrationMaker.Entities;
using IntegrationMaker.Service.InegrationServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationMaker.Service.InegrationServices.Concrete
{
   public class GetIntegrationService
    {
        public GetIntegrationService()
        { }
        public List<IntegrationModel> GetIntegrations()
        {
            var integrations = SelectQuery(Data.Get<Integration>().AsQueryable());
            return integrations.ToList();
        }

        private IQueryable<IntegrationModel> SelectQuery(IQueryable<Integration> organizations)
        {
            return organizations.Select(x => new IntegrationModel
            {
                Name = x.Name
            });
        }
    }
}
