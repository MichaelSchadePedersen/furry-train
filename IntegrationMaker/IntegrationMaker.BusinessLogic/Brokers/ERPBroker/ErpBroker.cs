using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using Toolpack.Shared.Entities;

//using static Toolpack.Shared.BusinessLogic.SqlFunctions;
//using static Toolpack.Shared.BusinessLogic.LogHelper;
////using static Toolpack.Shared.BusinessLogic.Utilities;
//using Toolpack.Shared.Entities.Erp;
//using System.Data.Entity.Core.Objects;
//using Toolpack.Shared.BusinessLogic.TaskConfiguration;
using System.Data;
using System.Linq.Expressions;

using IntegrationMaker.Entities;

namespace IntegrationMaker.BusinessLogic.Brokers.ERPBroker
{
    public abstract class ErpBroker
    {
        public enum ImportType
        {
            Full, Partial
        }

        protected static string erpTemplateDatabaseName { get; set; }
        protected static long erpTemplateDatabaseServerId { get; set; }

        protected static Dictionary<string, string> templateTableDefinitions = new Dictionary<string, string>(); //Name, definition
        public string ExternalSystemName { get; protected set; }

        public static readonly List<IntegrationType> IntegrationTypes = Data<IntegrationType>.Get();

        public Integration Integration { get; set; }
        public IntegrationType IntegrationType { get; set; }
        public IntegrationCredential Credential { get; set; }
        public Job Job { get; set; }
        //protected ImportType JobImportType { get; set; }
        //protected bool doFullImport;
        //protected string databaseName;
        //protected string targetConnectionString;

        //public List<tCompany> Companies { get; protected set; }
        //public List<tLedger> Ledgers { get; protected set; }

        //public List<tLedgerEntry> LedgerEntries { get; protected set; }
        //public List<tDistribution> Distributions { get; protected set; }
        //public List<tDistributionKey> DistributionKeys { get; protected set; }

        //public List<tItem> Items { get; protected set; }
        //public List<tItemGroup> ItemGroups { get; protected set; }
        //public List<tActivity> Activities { get; protected set; }
        //public List<tPriceGroup> PriceGroups { get; protected set; }
        //public List<tCurrency> Currencies { get; protected set; }
        //public List<tEmployee> Employees { get; protected set; }
        //public List<tCustomer> Customers { get; protected set; }
        //public List<tCustomerGroup> CustomerGroups { get; protected set; }
        //public List<tProject> Projects { get; protected set; }
        //public List<tProjectGroup> ProjectGroups { get; protected set; }
        //public List<tVendor> Vendors { get; protected set; }
        //public List<tVendorGroup> VendorGroups { get; protected set; }
        //public List<tSubscriber> Subscribers { get; protected set; }
        //public List<tSubscription> Subscriptions { get; protected set; }
        //public List<tProjectEntry> ProjectEntries { get; protected set; }
        //public List<tDimension> Dimensions { get; protected set; }
        //public List<tDimensionValue> DimensionValue { get; protected set; }
        //public List<tHeader> CustomerQuotations { get; protected set; }
        //public List<tHeaderLine> CustomerQuotationLines { get; protected set; }
        //public List<tHeader> CustomerOrders { get; protected set; }
        //public List<tHeaderLine> CustomerOrderLines { get; protected set; }
        //public List<tHeaderLine> VendorOrderLines { get; protected set; }
        //public List<tHeader> VendorOrder { get; protected set; }
        //public List<tHeader> CustomerInvoices { get; protected set; }
        //public List<tHeader> VendorInvoices { get; protected set; }
        //public List<tHeaderLine> CustomerInvoiceLines { get; protected set; }
        static ErpBroker()
        {

        }

        protected ErpBroker()
        {

        }

        //Full import
        public abstract Task PerformImportAsync(ImportType importType);
        //protected abstract Task GetCompanyAsync();
        //protected abstract Task GetLedgersAsync();
        //protected abstract Task GetDimensionsAsync();
        //protected abstract Task GetDimensionValuesAsync();
        //protected abstract Task GetLedgerEntriesAsync(DateTime? since);
        //protected abstract Task GetDistributionKeysAsync();
        //protected abstract Task GetDistributionsAsync();
        //protected abstract Task GetItemsAsync();
        //protected abstract Task GetItemGroupsAsync();
        //protected abstract Task GetActivitiesAsync();
        //protected abstract Task GetPriceGroupsAsync();
        //protected abstract Task GetCurrenciesAsync();
        //protected abstract Task GetEmployeesAsync();
        //protected abstract Task GetCustomersAsync();
        //protected abstract Task GetCustomerGroupsAsync();
        //protected abstract Task GetProjectsAsync();
        //protected abstract Task GetProjectGroupsAsync();
        //protected abstract Task GetVendorsAsync();
        //protected abstract Task GetVendorGroupsAsync();
        //protected abstract Task GetSubscribersAsync();
        //protected abstract Task GetSubscriptionsAsync();
        //public abstract Task GetProjectEntriesAsync();
        //public abstract Task GetQuotationsAsync();
        //public abstract Task GetQuotationLinesAsync();
        //public abstract Task GetOrdersAsync();
        //public abstract Task GetOrderLinesAsync();
        //public abstract Task GetInvoicesAsync();
        //public abstract Task GetInvoiceLinesAsync();

        public abstract Task VerifyIntegration();

        public static ErpBroker GetInstance(Integration integration, IntegrationCredential credential, Job job)
        {
            var integrationType = IntegrationTypes.Single(x => x.Id == integration.IntegrationTypeId);

            var assembly = Assembly.GetExecutingAssembly();

            var type = assembly.GetTypes().First(t => t.Name == "EconomicBroker");

            var broker = Activator.CreateInstance(type) as ErpBroker;

            broker.IntegrationType = integrationType;
            broker.Integration = integration;
            broker.Credential = credential;
            broker.Job = job;

            return broker;
        }

        public async Task InitializeAsync()
        {
            // Log($"Initializing import for integration ID {Integration.Id}. Marking integration as running.");
            await LogActionOnJob($"Initializing import for integration ID {Integration.Id}. Marking integration as running.");

            //Integration.IsRunning = true;
            await Data.SaveAsync(Integration).ConfigureAwait(false);

            //Log($"Ensuring target data structure.");

            //var organization = await Data<Organization>.GetAsync(Integration.OrganizationId).ConfigureAwait(false);
            //var databaseServer = await Data<DatabaseServer>.GetAsync(Integration.DatabaseServerId).ConfigureAwait(false);

            //databaseName = String.Format("ToolpackDatabase_{0}", organization.AccountNumber);

            //targetConnectionString = BuildConnectionString(databaseServer, databaseName, false);
            //await LogActionOnJob($"Create Database If Needed");

            //await SqlFunctions.CreateDatabaseIfNeededAsync(databaseServer, databaseName, organization);
            //await SqlFunctions.CreateSQLServerLoginIfNeededAsync(databaseServer, databaseName, organization);
            //await SqlFunctions.CreateSQLUserLoginIfNeededAsync(databaseServer, databaseName, organization);
        }

        public async Task InitDatawarehouse()
        {
            //await LogActionOnJob($"InitDatawarehouse");
            //int IncludeCasbookEntries = 0;
            //var db = ErpData.GetDbContext(targetConnectionString);
            //DatabaseServer databaseServer = Data.Get<DatabaseServer>(y => y.Id == Integration.DatabaseServerId).SingleOrDefault();
            //using (var connection = new SqlConnection(BusinessLogic.SQLHelper.SQLFunctions.BuildConnectionString(databaseServer, databaseName)))
            //{
            //    try
            //    {
            //        try
            //        {
            //            var includeCasbookEntriesSetting = Data.Get<IntergrationSetting>(x => x.IntergrationId == Integration.Id && x.settingsid == 2).SingleOrDefault();
            //            if (includeCasbookEntriesSetting != null)
            //            {
            //                if ((bool.Parse(includeCasbookEntriesSetting.Value)))
            //                    IncludeCasbookEntries = 1;
            //            }
            //            else
            //                IncludeCasbookEntries = 0;
            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //        SqlCommand trigerCalculations = new SqlCommand();
            //        trigerCalculations.CommandTimeout = 60000;

            //        trigerCalculations.CommandText = "tpsys.BuildCalculatedBalances";
            //        trigerCalculations.CommandType = CommandType.StoredProcedure;

            //        SqlParameter outPutParameterErrormessage = new SqlParameter();
            //        outPutParameterErrormessage.ParameterName = "@errormessage";
            //        outPutParameterErrormessage.Direction = System.Data.ParameterDirection.Output;
            //        outPutParameterErrormessage.SqlDbType = SqlDbType.NVarChar;
            //        outPutParameterErrormessage.Size = 100;
            //        outPutParameterErrormessage.Value = "What?";

            //        SqlParameter company = new SqlParameter();
            //        company.ParameterName = "@CompanyId";
            //        company.Direction = System.Data.ParameterDirection.Input;
            //        company.SqlDbType = SqlDbType.NVarChar;
            //        company.Size = 100;
            //        company.Value = Integration.Id.ToString();

            //        SqlParameter inCludeCashbookEntries = new SqlParameter();
            //        inCludeCashbookEntries.ParameterName = "@cashbookEntries";
            //        inCludeCashbookEntries.Direction = System.Data.ParameterDirection.Input;
            //        inCludeCashbookEntries.SqlDbType = SqlDbType.Bit;
            //        inCludeCashbookEntries.Value = IncludeCasbookEntries;


            //        SqlParameter outPutParameterResult = new SqlParameter();
            //        outPutParameterResult.ParameterName = "@result";

            //        outPutParameterResult.Direction = System.Data.ParameterDirection.Output;
            //        outPutParameterResult.Value = 0;
            //        trigerCalculations.Parameters.Add(company);
            //        trigerCalculations.Parameters.Add(inCludeCashbookEntries);
            //        trigerCalculations.Parameters.Add(outPutParameterErrormessage);
            //        trigerCalculations.Parameters.Add(outPutParameterResult);
            //        trigerCalculations.Connection = connection;
            //        connection.Open();
            //        trigerCalculations.ExecuteNonQuery();
            //        var errorMessage = Convert.ToString(trigerCalculations.Parameters["@errormessage"].Value);
            //        connection.Close();
            //        await LogActionOnJob($"InitDatawarehouse - BuildCalculatedBalances message: {errorMessage}");
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //};

        }


        public async Task FinalizeAsync(ImportType importType)
        {
            //await LogActionOnJob($"Begin finalize import for integration ID {Integration.Id}.");
            //Integration.IsRunning = false;
            //Integration.FullImport = false;
            //Integration.IncrementalImport = false;
            //await Data.SaveAsync(Integration).ConfigureAwait(false);
            //await LogActionOnJob($"End finalize import for integration ID {Integration.Id}. Marking integration as running: {Integration.IsRunning}, fullImport: {Integration.FullImport}, incrementalImport: {Integration.IncrementalImport}");
        }



        protected async Task LogActionOnJob(string actionLog, bool isWarning = false, bool isError = false, bool displayForUser = false)
        {
            //if (this.Job != null)
            //    this.Job.AddLog(actionLog, isWarning, isError, displayForUser);
        }

        #region BulkSave
        /// <summary>
        /// Bulksaves with the inclusion of an expression for data-cleanup
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="data">Data to be saved</param>
        /// <param name="CleanupExp">The expression used for filtering data to be deleted</param>
        /// <param name="useBatches">Save using batches</param>
        /// <returns>awaitable Task</returns>
        //protected async Task<IEnumerable<T>> BulkSave<T>(IEnumerable<T> data, Expression<Func<T, bool>> CleanupExp, bool useBatches = false) where T : class
        //{
        //    if (CleanupExp != null)
        //    {
        //        await DeleteBatch<T>(CleanupExp);
        //    }

        //    return await BulkSaveNoDelete<T>(data, useBatches);
        //}
        /// <summary>
        /// Bulksaves with the inclusion of an "ExternalRecordId" value used for data-cleanup
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="data">Data to be saved</param>
        /// <param name="deleteFrom">The ExternalRecordId that will limit the deletion of existing data - Note: the entry with the provided Id value will also be deleted (using ">=")</param>
        /// <param name="useBatches">Save using batches</param>
        /// <returns>awaitable Task</returns>
        //protected async Task<IEnumerable<T>> BulkSave<T>(IEnumerable<T> data, int deleteFrom, string tColumnName = "ExternalRecordId", tHeaderTypes headerType = tHeaderTypes.NA, bool useBatches = false) where T : class
        //{
        //   await DeleteBatch<T>(deleteFrom, headerType, tColumnName);

        //    return await BulkSaveNoDelete<T>(data, useBatches);
        //}
        /// <summary>
        /// Bulksaves with the inclusion of a "DocumentDate" value used for data-cleanup
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="data">Data to be saved</param>
        /// <param name="deleteFrom">The DocumentDate that will limit the deletion of existing data - Note: entries with the provided Date value will also be deleted (using ">="). Also note that only the "Date" part is used and not "Time"</param>
        /// <param name="useBatches">Save using batches</param>
        /// <returns>awaitable Task</returns>
        //protected async Task<IEnumerable<T>> BulkSave<T>(IEnumerable<T> data, DateTime deleteFrom, bool useBatches = false) where T : class
        //{
        //    await DeleteBatch<T>(deleteFrom);

        //    return await BulkSaveNoDelete<T>(data, useBatches);
        //}
        ///// <summary>
        ///// Bulksaves of data - will delete all entries from table from this integration
        ///// </summary>
        ///// <typeparam name="T">Class</typeparam>
        ///// <param name="data">Data to be saved</param>
        ///// <param name="tableName">Deprecated and no longer applies (uses )</param>
        ///// <param name="useBatches">Save using batches</param>
        ///// <returns></returns>
        //protected async Task<IEnumerable<T>> BulkSave<T>(IEnumerable<T> data, string tableName, bool useBatches = false) where T : class
        //{
        //    await DeleteBatch<T>();
        //    return await BulkSaveNoDelete(data, useBatches);
        //}

        //public async Task<IEnumerable<T>> BulkSaveNoDelete<T>(IEnumerable<T> data, bool useBatches) where T : class
        //{
        //    LogHelper.Log($"BulkSave {typeof(T).Name}");
        //    await LogActionOnJob($"BulkSave {typeof(T).Name}");

        //    if (useBatches)
        //    {
        //        ErpData.PerformBulkCopyInternal<T>(data, targetConnectionString, typeof(T).Name);
        //        await AddTransferDetails(typeof(T).Name, data.Count());
        //        return data;
        //    }
        //    else
        //    {
        //        var saveResult = ErpData.BulkSave(data, targetConnectionString);
        //        await AddTransferDetails(typeof(T).Name, saveResult.Count());
        //        return saveResult;
        //    }
        //}
        //#endregion

        //protected async Task DeleteBatch<T>() where T : class
        //{
        //    LogHelper.Log($"Cleaning up nonpersistant data: {typeof(T).Name}");
        //    await LogActionOnJob($"Cleaning up nonpersistant data: {typeof(T).Name}");

        //    var sql = $"DELETE FROM [tpdb].{typeof(T).Name} " +
        //        $"WHERE CompanyId = {this.Integration.Id} ";

        //    ErpData.ExecuteSql(this.targetConnectionString, sql);
        //}
        //protected async Task DeleteBatch<T>(Expression<Func<T, bool>> where) where T : class
        //{
        //    LogHelper.Log($"Cleaning up nonpersistant data: {typeof(T).Name}");
        //    await LogActionOnJob($"Cleaning up nonpersistant data: {typeof(T).Name}");

        //    ErpData.DeleteBatch<T>(this.targetConnectionString, where);
        //}
        //protected async Task DeleteBatch<T>(Int32 deleteFrom, tHeaderTypes headerType, string colName) where T : class
        //{
        //    LogHelper.Log($"Cleaning up nonpersistant data: {typeof(T).Name}");
        //    await LogActionOnJob($"Cleaning up nonpersistant data: {typeof(T).Name}");

        //    var sql = $"DELETE FROM [tpdb].{typeof(T).Name} " +
        //        $"WHERE CompanyId = {this.Integration.Id} " +
        //        $@"AND CASE WHEN {colName} NOT LIKE '%[^0-9]%' THEN CAST({colName} AS INT) ELSE -1 END >= {deleteFrom}" +
        //        (headerType == tHeaderTypes.NA ? "" : $"AND HeaderTypeId = {(int)headerType} ");

        //    ErpData.ExecuteSql(this.targetConnectionString, sql);
        //}
        //protected async Task DeleteBatch<T>(DateTime deleteFrom) where T : class
        //{
        //    LogHelper.Log($"Cleaning up nonpersistant data: {typeof(T).Name}");
        //    await LogActionOnJob($"Cleaning up nonpersistant data: {typeof(T).Name}");

        //    var sql = $@"DELETE FROM [tpdb].{typeof(T).Name} " +
        //        $"WHERE CompanyId = {this.Integration.Id} " +
        //        $"AND DocumentDate >= {deleteFrom.ToString("yyyy-MM-dd")}";

        //    ErpData.ExecuteSql(this.targetConnectionString, sql);
        //}

        //private async Task AddTransferDetails(string tableName, int rowCount)
        //{
        //    if (this.Job == null)
        //        return;

        //    await Data<JobTransferDetail>.SaveAsync(new JobTransferDetail()
        //    {
        //        JobId = this.Job.Id,
        //        TableName = tableName,
        //        RowsTransferred = rowCount
        //    });
        //}

        //protected class BulksaveQuery<T>
        //{
        //    public Expression<Func<T, bool>> CleanupExpression { get; private set; }

        //    public BulksaveQuery(Expression<Func<T, bool>> exp)
        //    {
        //        this.CleanupExpression = exp;
        //    }
        //}
    }
}
#endregion
