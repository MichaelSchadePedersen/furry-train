using IntegrationMaker.BusinessLogic.TaskConfiguration;
using IntegrationMaker.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IntegrationMaker.BusinessLogic.Tasks.Integration
{
    public abstract class ImportBase : ProvisioningTask
    {
        public ImportBase(Job job) : base(job)
        {
        }

        public override abstract Task<ProvisioningResult> ExecuteJobAsync();

        public async Task AddTransferDetailsAsync(string tableName, int rowsTransferred)
        {
            await Data<JobTransferDetail>.SaveAsync(new JobTransferDetail()
            {
                JobId = this.Job.Id,
                TableName = tableName,
                RowsTransferred = rowsTransferred
            });
        }

        public async override Task StartAsync()
        {
            //Update integration
            var integration = await Data<IntegrationMaker.Entities.Integration>.GetAsync(this.Job.TargetId);

            //Check if Integration is running, meaning that another job for this Integration has been started (should not happen, but just in case..)
            if (integration.IsRunning)
            {
                //    throw new AccessViolationException("Integration was found to be already running when trying to start importjob");
                this.Job.AddLog("Integration was found to be already running when trying to start importjob - postponing execution", true);
                return;
            }



            //Set integration to IsRunning, stating that a job is now running for this Integration
            integration.IsRunning = true;
            await Data<IntegrationMaker.Entities.Integration>.SaveAsync(integration);

            //Mark job as having started
            this.Job.Start();
        }

        public async override Task CompleteAsync(string message = "")
        {
            var integration = Data<IntegrationMaker.Entities.Integration>.Get(this.Job.TargetId);
            var integrationType = Data<IntegrationMaker.Entities.IntegrationType>.Get(x => x.Id == integration.IntegrationTypeId).SingleOrDefault();
            integration.IsRunning = false;
            var updatedIntegrations = Data.Get<IntegrationMaker.Entities.Integration>(item => (item.LastIncrementalDataImortUTC != null || item.LastFullDataImortUTC != null ) && item.CompanyId ==integration.CompanyId).ToList();
            bool IsFirstRun = (integration.LastFullDataImortUTC == null && integration.LastIncrementalDataImortUTC == null && updatedIntegrations.Count==0) ? true : false;
            if(this.Job.JobTypeId == (int)JobTypes.PartialIntegrationImport)
            integration.LastIncrementalDataImortUTC = DateTime.Now;
            if (this.Job.JobTypeId == (int)JobTypes.FullIntegrationImport)
                integration.LastFullDataImortUTC= DateTime.Now;
            integration.ErrorCode = 0;
            await Data<IntegrationMaker.Entities.Integration>.SaveAsync(integration);

            this.Job.Complete(message);

            //if NotifyWhenComplete - notify subscriber as well

            if (IsFirstRun)
            {

                var subject = string.Empty;//string.Format("Freeyournumbers – data is ready for {0}", Data.Get<Organization>(x => x.Id == integration.OrganizationId).First().Name);

                string body = string.Empty;
                //using streamreader for reading my htmltemplate   
                var appDomain = System.AppDomain.CurrentDomain;
                var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;

                using (StreamReader reader = new StreamReader(Path.Combine(basePath, "Email templates", "DataImportFirstRun.html")))
                {

                    body = reader.ReadToEnd();

                }

                //body = body.Replace("{Kundenavn}", Integration.Get<Company>(x => x.Id == integration.c).First().Name); //replacing the required things  

                body = body.Replace("{IntegrationName}", integration.Name);

                body = body.Replace("{IntegrationNumber}", integration.Id.ToString());

                body = body.Replace("{IntegrationType}", integrationType.Name);

                body = body.Replace("{UpdateTime}", DateTime.Now.ToString());


                NotifyFreeYourNumbersThatDataIsImportedAtFirstRunAsync(subject, body);
            }
        
    
            if (this.Job.NotifyWhenComplete)
            {
                var subscriber = Data.Get<CompanyUser>(x => x.Id == this.Job.UserId).SingleOrDefault();
                //var user = Authentication.GetUser(subscriber.ObjectId);

                var subject = "Freeyournumbers – data is ready";

                string body = string.Empty;
                //using streamreader for reading my htmltemplate   
                var appDomain = System.AppDomain.CurrentDomain;
                var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
           
                using (StreamReader reader = new StreamReader(Path.Combine(basePath, "Email templates", "DataImportCompleted.html")))
                {

                    body = reader.ReadToEnd();

                }

                //body = body.Replace("{subscriber.Name}", user.DisplayName); //replacing the required things  

                body = body.Replace("{IntegrationName}", integration.Name);

                body = body.Replace("{IntegrationNumber}", integration.Id.ToString());

                body = body.Replace("{IntegrationType}", integrationType.Name);

                body = body.Replace("{UpdateTime}", DateTime.Now.ToString());

               
                //NotifySubscriberAsync(subject, body, user.Email);
            }
                
        }

        public async override Task FailAsync(string errorMessage)
        {
            await FailAsync(errorMessage, ErpIntegrationErrorCodes.Unknown);
        }

        public async override Task FailAsync(string errorMessage, ErpIntegrationErrorCodes errorCode)
        {
            var integration = Data<IntegrationMaker.Entities.Integration>.Get(this.Job.TargetId);
            integration.IsRunning = false;
            integration.ErrorCode = (int)errorCode;
            await Data<IntegrationMaker.Entities.Integration>.SaveAsync(integration);

            this.Job.Fail(errorMessage, errorCode);

            SendMailNotification(errorMessage, errorCode);

            //if NotifyWhenComplete - notify subscriber as well
            if (this.Job.NotifyWhenComplete)
            {
                var sub = $"Jobtype: {this.GetType().Name} running on integration \"{integration.Name}\" has failed with the errorcode \"{errorCode.ToString()}\"";
                var body = sub;
                NotifySubscriberAsync(sub, body, "GetSubscriberEmailAndInsertItHere");
            }
        }

        public async Task SendMailNotification(string errorMessage, ErpIntegrationErrorCodes errorCode)
        {
            //Send the mail
            var subject = $"Jobtype: {this.GetType().Name}, Integration {this.Job.TargetId} - ErrorCode: {errorCode.ToString()}";
            var body = errorMessage;

            SendMailNotificationAsync(subject, body);
        }
    }
}
