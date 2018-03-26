using IntegrationMaker.BusinessLogic.Brokers.ERPBroker;
using IntegrationMaker.BusinessLogic.TaskConfiguration;
using IntegrationMaker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.BusinessLogic.Tasks.Integration
{
    public class ImportFull : ImportBase
    {
        public ImportFull(Job job) : base(job)
        {
        }

        public override async Task<ProvisioningResult> ExecuteJobAsync()
        {
            var integration = await Data<IntegrationMaker.Entities.Integration>.GetAsync(this.Job.TargetId, x => x.IntegrationCredentials);

            var broker = ErpBroker.GetInstance(integration, integration.IntegrationCredentials.First(), this.Job);
            await broker.InitializeAsync();
            await broker.PerformImportAsync(ErpBroker.ImportType.Full);
            await broker.InitDatawarehouse();
            await broker.FinalizeAsync(ErpBroker.ImportType.Full);

            return new ProvisioningResult() { OK = true, Message = "Import successful" };
        }
    }
}
