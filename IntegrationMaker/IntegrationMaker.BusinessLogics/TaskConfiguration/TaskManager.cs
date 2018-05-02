using IntegrationMaker.BusinessLogic.TaskConfiguration;
using IntegrationMaker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.BusinessLogic.TaskConfiguration
{
    public class TaskManager
    {
        private static List<JobTypes> IntegrationJobTypes = new List<JobTypes>() { JobTypes.PartialIntegrationImport, JobTypes.FullIntegrationImport };

        public static async Task<List<ProvisioningTask>> GetUncompletedJobs(int take)
        {
            var taskList = new List<ProvisioningTask>();

            //get pending jobs
            var jobRequests = await Data<Job>.GetAsync(x => x.JobStateId == (int)JobStates.Created, x => x.JobTypeId).ConfigureAwait(false);
            
            //get already running jobs
            var ongoingJobs = await Data<Job>.GetAsync(x => x.JobStateId == (int)JobStates.InProcess, x => x.JobTypeId).ConfigureAwait(false);

            //determine which integrations are already running an IntegrationImportJob (we dont want to start a new import job if one is already running)
            var integrationsWithOngoingJobs = ongoingJobs.Where(x => IntegrationJobTypes.Any(y => (int)y == x.JobTypeId)).Select(x => x.TargetId).Distinct();

            ////remove all import-jobs belonging to an integration that already has an import-job running
            //jobRequests.RemoveAll(jobRequest => integrationsWithOngoingJobs.Any(integrationId => integrationId == jobRequest.TargetId) && IntegrationJobTypes.Any(jobType => (int)jobType == jobRequest.JobTypeId));

            var integrations = await Data<Integration>.GetAsync(x => integrationsWithOngoingJobs.Contains(x.Id));
            var organizationsWithJobsRunning = integrations.Select(x => x.CompanyId);
            var integrationsUnderRunningOrganization = (await Data<Integration>.GetAsync(x => organizationsWithJobsRunning.Any(y => y == x.CompanyId))).Select(x => x.Id);

            jobRequests.RemoveAll(jobRequest => integrationsUnderRunningOrganization.Any(integrationId => integrationId == jobRequest.TargetId) && IntegrationJobTypes.Any(jobType => (int)jobType == jobRequest.JobTypeId));

            //remove all but the first job for each organization (we don't want to start more than 1 import job pr. organization)
            var integrationsWithQueuedJobs = jobRequests.Where(x => IntegrationJobTypes.Any(y => (int)y == x.JobTypeId)).Select(x => x.TargetId).Distinct();
            var integrationsInQueue = await Data<Integration>.GetAsync(x => integrationsWithQueuedJobs.Contains(x.Id));
            var organizationsinQueue = integrationsInQueue.Select(x => x.CompanyId);

            foreach (var orgId in organizationsinQueue)
            {
                var myIntegrations = integrationsInQueue.Where(x => x.CompanyId == orgId);
                var myJobs = jobRequests.Where(x => myIntegrations.Select(y => y.Id).Any(z => z == x.TargetId));
                if(myJobs.Count() > 1)
                {
                    jobRequests.RemoveAll(x => myJobs.Skip(1).Contains(x));
                }
            }

            var jobMappingTasks = jobRequests.Select(x => ResolveJobRequestHandler(x));
            Task<ProvisioningTask>.WaitAll(jobMappingTasks.ToArray());

            return jobMappingTasks.Select(x => x.Result).OrderBy(x => x.Job.JobTypeId == (int)JobTypes.PartialIntegrationImport).Take(take).ToList();
            //return jobMappingTasks.Select(x => x.Result).OrderBy(x => x.Job.JobTypeId == (int)JobTypes.PartialIntegrationImport).ToList();
        }

        public static async Task<ProvisioningTask> ResolveJobRequestHandler(Job jobRequest)
        {
            switch((JobTypes)jobRequest.JobTypeId)
            {
                case JobTypes.FullIntegrationImport:
                    return new Tasks.Integration.ImportFull(jobRequest);

                case JobTypes.PartialIntegrationImport:
                    return new Tasks.Integration.ImportPartial(jobRequest);

                default:
                    break;
            }
            throw new ApplicationException("Job type not assigned to handler");
        }
    }
}
