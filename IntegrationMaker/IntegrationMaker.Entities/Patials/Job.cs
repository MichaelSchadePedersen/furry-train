using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.Entities
{
    public partial class Job
    {
        public void Start()
        {
            this.JobStateId = (int)JobStates.InProcess;
            this.UTCStarted = DateTime.UtcNow;
            Data<Job>.SaveAsync(this).Wait();
        }

        public void Fail(String errorMessage)
        {
            using(var db = new IntegrationMakerEntities())
            {
                var me = db.Jobs.Single(x => x.Id == this.Id);

                if ((JobStates)me.JobStateId == JobStates.Failed)
                    return;

                me.JobStateId = (int)JobStates.Failed;
                me.UTCFinished = DateTime.UtcNow;

                //add errorlog
                me.JobLogs.Add(new JobLog()
                {
                    JobId = this.Id,
                    Information = errorMessage,
                    UTCCreated = DateTime.UtcNow,
                    IsWarning = false,
                    IsError = true,
                    DisplayForUser = false,
                });
                db.SaveChanges();
            }
        }

        public void Fail(String errorMessage, ErpIntegrationErrorCodes errorCode)
        {
            using (var db = new IntegrationMakerEntities())
            {
                var me = db.Jobs.Single(x => x.Id == this.Id);

                if ((JobStates)me.JobStateId == JobStates.Failed)
                    return;

                me.JobStateId = (int)JobStates.Failed;
                me.UTCFinished = DateTime.UtcNow;
                me.ErpIntegrationErrorCode = (int)errorCode;

                //add errorlog
                me.JobLogs.Add(new JobLog()
                {
                    JobId = this.Id,
                    Information = errorMessage,
                    UTCCreated = DateTime.UtcNow,
                    IsWarning = false,
                    IsError = true,
                    DisplayForUser = false,
                });
                db.SaveChanges();
            }
        }

        public void Complete(String completeMessage = "")
        {
            using (var db = new IntegrationMakerEntities())
            {
                var me = db.Jobs.Single(x => x.Id == this.Id);

                //if jobState is already complete - do nothing, else set complete state
                if ((JobStates)me.JobStateId == JobStates.Completed)
                    return;

                me.JobStateId = (int)JobStates.Completed;
                me.UTCFinished = DateTime.UtcNow;
                //add log entry
                me.JobLogs.Add(new JobLog()
                {
                    JobId = this.Id,
                    Information = completeMessage,
                    UTCCreated = DateTime.UtcNow,
                    IsWarning = false,
                    IsError = false,
                    DisplayForUser = false,
                });
                db.SaveChanges();
            }
        }

        public void AddLog(String actionData, bool warning = false, bool error = false, bool displayToUser = false)
        {
            using (var db = new IntegrationMakerEntities())
            {
                var me = db.Jobs.Single(x => x.Id == this.Id);

                //add log entry
                me.JobLogs.Add(new JobLog()
                {
                    JobId = this.Id,
                    Information = actionData,
                    UTCCreated = DateTime.UtcNow,
                    IsWarning = warning,
                    IsError = error,
                    DisplayForUser = displayToUser,
                });
                db.SaveChanges();
            }
        }
    }
}
