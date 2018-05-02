using IntegrationMaker.BusinessLogic.Brokers.CRMBroker;
using IntegrationMaker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.Services.CRMServices.Concrete
{
    public class InsertRecordService
    {
        public InsertRecordService()
        { }
        public async System.Threading.Tasks.Task<LeadModel> CreateLeadAsync(LeadModel leadModel)
        {
            var broker = new ZohoCRMBroker();
            broker.CreateLead(leadModel);
            return leadModel;
        }
    }
}
