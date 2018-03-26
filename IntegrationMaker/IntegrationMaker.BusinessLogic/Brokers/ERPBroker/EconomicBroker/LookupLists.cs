using System.Linq;
using System;
using System.Collections.Generic;
using EconomicSoapService;
using IntegrationMaker.BusinessLogic.Brokers.ERPBroker.EconomicBroker;
using static IntegrationMaker.BusinessLogic.Brokers.ERPBroker.ErpBroker;
using Toolpack.Shared.BusinessLogic;

namespace IntegrationMaker.BusinessLogic.Brokers.ERPBroker.EconomicBroker
{
    public class LookupLists
    {
        private EconomicWebServiceSoapClient Session { get; set; }

        public Dictionary<CompanyHandle, CompanyData> Companies { get; set; }
        public Dictionary<AccountHandle, AccountData> Accounts { get; set; }
        public Dictionary<AccountingPeriodHandle, AccountingPeriodData> AccountingPeriods { get; set; }
        public Dictionary<DebtorHandle, DebtorData> Debitors { get; set; }
        public Dictionary<CreditorHandle, CreditorData> Creditors { get; set; }
        public Dictionary<DepartmentHandle, DepartmentData> Departments { get; set; }
        public Dictionary<EmployeeHandle, EmployeeData> Employees { get; set; }
        public Dictionary<EmployeeGroupHandle, EmployeeGroupData> EmployeeGroups { get; set; }
        public Dictionary<OrderHandle, OrderData> Orders { get; set; }
        public Dictionary<OrderLineHandle, OrderLineData> OrderLines { get; set; }
        public Dictionary<QuotationHandle, QuotationData> Quotations { get; set; }
        public Dictionary<QuotationLineHandle, QuotationLineData> QuotationLines { get; set; }
        public Dictionary<SubscriptionHandle, SubscriptionData> Subscriptions { get; set; }
        public Dictionary<ProductHandle, ProductData> Products { get; set; }
        public Dictionary<ProjectHandle, ProjectData> Projects { get; set; }
        public Dictionary<ProjectGroupHandle, ProjectGroupData> ProjectGroups { get; set; }
        public Dictionary<ProductGroupHandle, ProductGroupData> ProductGroups { get; set; }
        public Dictionary<AccountingYearHandle, AccountingYearData> AccountingYears { get; set; }
        public Dictionary<BudgetFigureHandle, BudgetFigureData> BudgetFigures { get; set; }
        public Dictionary<CurrencyHandle, CurrencyData> Currencies { get; set; }
        public Dictionary<TemplateCollectionHandle, TemplateCollectionData> TemplateCollections { get; set; }
        public Dictionary<DebtorContactHandle, DebtorContactData> DebtorContacts { get; set; }
        public Dictionary<PriceGroupHandle, PriceGroupData> PriceGroups { get; set; }
        public Dictionary<InventoryLocationHandle, InventoryLocationData> InventoryLocation { get; set; }
        public Dictionary<CreditorContactHandle, CreditorContactData> CreditorContacts { get; set; }
        public Dictionary<DebtorGroupHandle, DebtorGroupData> DebtorGroups { get; set; }
        public Dictionary<CreditorGroupHandle, CreditorGroupData> CreditorGroups { get; set; }
        public Dictionary<BankPaymentTypeHandle, BankPaymentTypeData> BankPaymentTypes { get; set; }
        public Dictionary<TermOfPaymentHandle, TermOfPaymentData> TermsOfPayment { get; set; }
        public Dictionary<DistributionKeyHandle, DistributionKeyData> DistributionKeys { get; set; }
        public Dictionary<DistributionKeyHandle, DistributionKeyData> DistributionKeys_Departments { get; set; }
        public Dictionary<UnitHandle, UnitData> Units { get; set; }
        public Dictionary<DeliveryLocationHandle, DeliveryLocationData> DeliveryLocations { get; set; }
        public Dictionary<CurrentSupplierInvoiceHandle, CurrentSupplierInvoiceData> CurentSupplierInvoices { get; set; }
     
        public Dictionary<VatAccountHandle, VatAccountData> VatAccounts { get; set; }
        public Dictionary<CashBookHandle, CashBookData> CashBooks { get; set; }
        public Dictionary<CashBookEntryHandle, CashBookEntryData> CashBooksEntries { get; set; }
        public Dictionary<CostTypeHandle, CostTypeData> CostTypes { get; set; }
        public Dictionary<CostTypeGroupHandle, CostTypeGroupData> CostTypeGroups { get; set; }
        public Dictionary<InvoiceHandle, InvoiceData> Invoices { get; set; }
        public Dictionary<ActivityHandle, ActivityData> Activities { get; set; }
        public Dictionary<TimeEntryHandle, TimeEntryData> TimeEntries { get; set; }
        public Dictionary<SubscriberHandle, SubscriberData> Subscribers { get; set; }
        public Dictionary<Type, Dictionary<object, object>> TypeLookup { get; private set; }
        public Dictionary<EntryHandle, EntryData> Entries { get; set; }
        
        public IDictionary<Type, ISessionUnitOfWork<object, object>> EntryFunctions;

        public LookupLists(EconomicWebServiceSoapClient session)
        {
            this.Session = session;
            EntryFunctions = new Dictionary<Type, ISessionUnitOfWork<object, object>>();
        }
        private DepartmentHandle[] GetDistributionDepartment(DistributionKeyHandle handle)
        {

            return Session.DistributionKey_GetDepartments(handle);
        }
        public EconomicSoapService.ProductPriceHandle[] GetProductPriceHandles()
        {
            return Session.ProductPrice_FindByProductList(Session.Product_GetAll());
        }
        public EconomicSoapService.CashBookEntryHandle[] getAllCashBookEntryHandles()
        {
            List<CashBookEntryHandle> list = new List<CashBookEntryHandle>();
            var cashbooks = Session.CashBook_GetAll();
            foreach (var cashBookHandle in cashbooks)
            {
                var cashbookEntries = Session.CashBook_GetEntries(cashBookHandle);
                list.AddRange(cashbookEntries.ToList());
            }

            return list.ToArray();
        }
        public EconomicSoapService.CurrentSupplierInvoiceLineHandle[] GetCurrentSupplierInvoiceLine()
        {
            var currentSupplierInvoicehandles = Session.CurrentSupplierInvoice_GetAll();
            var currentSupplierInvoiceLines = new List<CurrentSupplierInvoiceLineHandle>();
            foreach (var currentSupplierInvoiceHandle in currentSupplierInvoicehandles)
            {
               currentSupplierInvoiceLines.AddRange(Session.CurrentSupplierInvoice_GetLines(currentSupplierInvoiceHandle).ToList());
            }
            return currentSupplierInvoiceLines.ToArray();
        }
        private QuotationHandle[] getQuotations_GetCurrent()
        {
            return Session.Quotation_GetAllCurrent();
        }
        private QuotationLineHandle[] getQuotationLines_GetAll()
        {
            return Session.QuotationLine_FindByQuotationList(Session.Quotation_GetAllCurrent());
        }
        private OrderHandle[] getOrders_GetCurrent()
        {
            return Session.Order_GetAllCurrent();
        }
        private OrderLineHandle[] getOrderLines_GetAll()
        {
            return Session.OrderLine_FindByOrderList(Session.Order_GetAllCurrent());
        }

        ////NOTE:<rk> This is primarily uasable for tests so there is no need to build entire lookup list to populate customers
        //public void BuildCustomersLookup()
        //{
        //    var api = new EconomicApiAccess();
        //    this.Companies = api.BuildDictionary(() => new[] { Session.Company_Get() }, Session.Company_GetDataArray);
        //    BuildEntryFunctions();
        //    this.TypeLookup = api.TypeLookup;
        //}

        private void BuildEntryFunctions(Brokers.ERPBroker.ErpBroker.ImportType importType)
        {
            this.EntryFunctions.Add(typeof(CurrentSupplierInvoiceLineHandle),
               new SessionUnitOfWork<CurrentSupplierInvoiceLineHandle, CurrentSupplierInvoiceLineData>(
                   (filter) => GetCurrentSupplierInvoiceLine(), Session.CurrentSupplierInvoiceLine_GetDataArray));

            //Quotations - full
            this.EntryFunctions.Add(typeof(QuotationHandle),
                new SessionUnitOfWork<QuotationHandle, QuotationData>(
                    (filter) => getQuotations_GetCurrent(), Session.Quotation_GetDataArray));

            //QuotationLines - full
            this.EntryFunctions.Add(typeof(QuotationLineHandle),
                new SessionUnitOfWork<QuotationLineHandle, QuotationLineData>(
                    (filter) => getQuotationLines_GetAll(), Session.QuotationLine_GetDataArray));
            //ProductPrice - full
            this.EntryFunctions.Add(typeof(ProductPriceHandle),
                new SessionUnitOfWork<ProductPriceHandle,ProductPriceData>(
                    (filter) => GetProductPriceHandles(), Session.ProductPrice_GetDataArray));

            //Orders - full
            this.EntryFunctions.Add(typeof(OrderHandle),
                new SessionUnitOfWork<OrderHandle, OrderData>(
                    (filter) => getOrders_GetCurrent(), Session.Order_GetDataArray));

            //OrderLines - full
            this.EntryFunctions.Add(typeof(OrderLineHandle),
                new SessionUnitOfWork<OrderLineHandle, OrderLineData>(
                    (filter) => getOrderLines_GetAll(), Session.OrderLine_GetDataArray));
            this.EntryFunctions.Add(typeof(EntryHandle),
                new SessionUnitOfWork<EntryHandle, EntryData>(
                    (filter) => Session.Entry_FindBySerialNumberInterval((int)filter, Int32.MaxValue),
                    Session.Entry_GetDataArray));

            this.EntryFunctions.Add(typeof(InvoiceHandle),
                new SessionUnitOfWork<InvoiceHandle, InvoiceData>(
                    (filter) => Session.Invoice_FindByNumberInterval((int)filter, Int32.MaxValue),
                    Session.Invoice_GetDataArray));

            this.EntryFunctions.Add(typeof(InvoiceLineHandle),
                new SessionUnitOfWork<InvoiceLineHandle, InvoiceLineData>(
                    (filter) => Session.InvoiceLine_FindByInvoiceNumberInterval((int)filter, Int32.MaxValue),
                    Session.InvoiceLine_GetDataArray));

            this.EntryFunctions.Add(typeof(DebtorEntryHandle),
                new SessionUnitOfWork<DebtorEntryHandle, DebtorEntryData>(
                    (filter) => Session.DebtorEntry_FindBySerialNumber((int)filter, Int32.MaxValue),
                    Session.DebtorEntry_GetDataArray));

            this.EntryFunctions.Add(typeof(CreditorEntryHandle),
                new SessionUnitOfWork<CreditorEntryHandle, CreditorEntryData>(
                    (filter) => Session.CreditorEntry_FindBySerialNumber((int)filter, Int32.MaxValue),
                    Session.CreditorEntry_GetDataArray));

            this.EntryFunctions.Add(typeof(CashBookEntryHandle),
                new SessionUnitOfWork<CashBookEntryHandle, CashBookEntryData>(
                    (filter) => getAllCashBookEntryHandles(),
                    Session.CashBookEntry_GetDataArray));
        }
        public void BuildLookupList(ImportType importType)
        {
            var api = new EconomicApiAccess();

            DateTime updateDate = DateTime.MinValue;
            this.Projects = api.BuildDictionary(Session.Project_GetAll, Session.Project_GetDataArray);
            this.VatAccounts = api.BuildDictionary(Session.VatAccount_GetAll, Session.VatAccount_GetDataArray);
            this.DebtorContacts = api.BuildDictionary(Session.DebtorContact_GetAll, Session.DebtorContact_GetDataArray);
            this.DeliveryLocations = api.BuildDictionary(Session.DeliveryLocation_GetAll, Session.DeliveryLocation_GetDataArray);
            this.TemplateCollections = api.BuildDictionary(Session.TemplateCollection_GetAll, Session.TemplateCollection_GetDataArray);
            this.TermsOfPayment = api.BuildDictionary(Session.TermOfPayment_GetAll, Session.TermOfPayment_GetDataArray);
            this.EmployeeGroups = api.BuildDictionary(Session.EmployeeGroup_GetAll, Session.EmployeeGroup_GetDataArray);
            this.Employees = api.BuildDictionary(Session.Employee_GetAll, Session.Employee_GetDataArray);

            this.Companies = api.BuildDictionary(() => new[] { Session.Company_Get() }, Session.Company_GetDataArray);
            this.AccountingPeriods = api.BuildDictionary(Session.AccountingPeriod_GetAll, Session.AccountingPeriod_GetDataArray);
            this.AccountingYears = api.BuildDictionary(Session.AccountingYear_GetAll, Session.AccountingYear_GetDataArray);
            this.BudgetFigures = api.BuildDictionary(Session.BudgetFigure_GetAll, Session.BudgetFigure_GetDataArray);
            this.Accounts = api.BuildDictionary(Session.Account_GetAll, Session.Account_GetDataArray);
            this.DistributionKeys = api.BuildDictionary(Session.DistributionKey_GetAll, Session.DistributionKey_GetDataArray);

            this.Departments = api.BuildDictionary(Session.Department_GetAll, Session.Department_GetDataArray);
            this.Currencies = api.BuildDictionary(Session.Currency_GetAll, Session.Currency_GetDataArray);


            this.Creditors = api.BuildDictionary(Session.Creditor_GetAll, Session.Creditor_GetDataArray);
            this.Debitors = api.BuildDictionary(Session.Debtor_GetAll, Session.Debtor_GetDataArray);
            this.Products = api.BuildDictionary(Session.Product_GetAll, Session.Product_GetDataArray);

            this.DebtorGroups = api.BuildDictionary(Session.DebtorGroup_GetAll, Session.DebtorGroup_GetDataArray);
            this.CreditorGroups = api.BuildDictionary(Session.CreditorGroup_GetAll, Session.CreditorGroup_GetDataArray);
            this.ProductGroups = api.BuildDictionary(Session.ProductGroup_GetAll, Session.ProductGroup_GetDataArray);
            this.Units = api.BuildDictionary(Session.Unit_GetAll, Session.Unit_GetDataArray);
            this.PriceGroups = api.BuildDictionary(Session.PriceGroup_GetAll, Session.PriceGroup_GetDataArray);
            this.InventoryLocation = api.BuildDictionary(Session.InventoryLocation_GetAll, Session.InventoryLocation_GetDataArray);
            this.CurentSupplierInvoices = api.BuildDictionary(Session.CurrentSupplierInvoice_GetAll, Session.CurrentSupplierInvoice_GetDataArray);
           
            this.CreditorContacts = api.BuildDictionary(Session.CreditorContact_GetAll, Session.CreditorContact_GetDataArray);
            this.BankPaymentTypes = api.BuildDictionary(Session.BankPaymentType_GetAll, Session.BankPaymentType_GetDataArray);

            //this.Quotations = api.BuildDictionary(Session.Quotation_GetAll, Session.Quotation_GetDataArray);
            //this.QuotationLines = api.BuildDictionary(getQuotationLines_GetAll, Session.QuotationLine_GetDataArray);

            //this.Orders = api.BuildDictionary(Session.Order_GetAll, Session.Order_GetDataArray);
            //this.OrderLines = api.BuildDictionary(getOrderLines_GetAll, Session.OrderLine_GetDataArray);

            this.Subscriptions = api.BuildDictionary(Session.Subscription_GetAll, Session.Subscription_GetDataArray);
            this.Subscribers = api.BuildDictionary(Session.Subscriber_GetAll, Session.Subscriber_GetDataArray);

            this.CostTypeGroups = api.BuildDictionary(Session.CostTypeGroup_GetAll, Session.CostTypeGroup_GetDataArray);
            this.CostTypes = api.BuildDictionary(Session.CostType_GetAll, Session.CostType_GetDataArray);
            this.CashBooks = api.BuildDictionary(Session.CashBook_GetAll, Session.CashBook_GetDataArray);


            this.TimeEntries = api.BuildDictionary(Session.TimeEntry_GetAll, Session.TimeEntry_GetDataArray);
            this.ProjectGroups = api.BuildDictionary(Session.ProjectGroup_GetAll, Session.ProjectGroup_GetDataArray);
            this.Activities = api.BuildDictionary(Session.Activity_GetAll, Session.Activity_GetDataArray);

            BuildEntryFunctions(importType);

            ////Quotations
            //EntryFunctions.Add(typeof(QuotationHandle),
            //    new SessionUnitOfWork<QuotationHandle, QuotationData>(
            //        (filter) => Session.Quotation_FindByNumberInterval((int)filter, Int32.MaxValue),
            //        Session.Quotation_GetDataArray));
            ////QuotationLines
            //EntryFunctions.Add(typeof(QuotationLineHandle),
            //    new SessionUnitOfWork<QuotationLineHandle, QuotationLineData>(
            //        (filter) => Session.QuotationLine_FindByQuotationList((QuotationHandle[])filter),
            //        Session.QuotationLine_GetDataArray));

            ////Orders
            //EntryFunctions.Add(typeof(OrderHandle),
            //    new SessionUnitOfWork<OrderHandle, OrderData>(
            //        (filter) => Session.Order_FindByNumberInterval((int)filter, Int32.MaxValue),
            //            Session.Order_GetDataArray));

            ////OrderLines
            //EntryFunctions.Add(typeof(OrderLineHandle),
            //    new SessionUnitOfWork<OrderLineHandle, OrderLineData>(
            //        (filter) => Session.OrderLine_FindByOrderList((OrderHandle[])filter),
            //        Session.OrderLine_GetDataArray));

            //EntryFunctions.Add(typeof (EntryHandle),
            //    new SessionUnitOfWork<EntryHandle, EntryData>(
            //        (filter) => Session.Entry_FindBySerialNumberInterval((int)filter, Int32.MaxValue),
            //        Session.Entry_GetDataArray));

            //EntryFunctions.Add(typeof(InvoiceHandle),
            //    new SessionUnitOfWork<InvoiceHandle, InvoiceData>(
            //        (filter) => Session.Invoice_FindByNumberInterval((int)filter, Int32.MaxValue),
            //        Session.Invoice_GetDataArray));

            //EntryFunctions.Add(typeof(InvoiceLineHandle),
            //    new SessionUnitOfWork<InvoiceLineHandle, InvoiceLineData>(
            //        (filter) => Session.InvoiceLine_FindByInvoiceNumberInterval((int)filter, Int32.MaxValue),
            //        Session.InvoiceLine_GetDataArray));

            //EntryFunctions.Add(typeof(DebtorEntryHandle),
            //    new SessionUnitOfWork<DebtorEntryHandle, DebtorEntryData>(
            //        (filter) => Session.DebtorEntry_FindBySerialNumber((int)filter, Int32.MaxValue),
            //        Session.DebtorEntry_GetDataArray));

            //EntryFunctions.Add(typeof(CreditorEntryHandle),
            //    new SessionUnitOfWork<CreditorEntryHandle, CreditorEntryData>(
            //        (filter) => Session.CreditorEntry_FindBySerialNumber((int)filter, Int32.MaxValue),
            //        Session.CreditorEntry_GetDataArray));

            //EntryFunctions.Add(typeof(CashBookEntryHandle),
            //    new SessionUnitOfWork<CashBookEntryHandle, CashBookEntryData>(
            //        (filter) => getAllCashBookEntryHandles(),
            //        Session.CashBookEntry_GetDataArray));

            this.TypeLookup = api.TypeLookup;
        }
    }

    public interface ISessionUnitOfWork<out P, out D>
    {
        Dictionary<object, object> Execute(object filter);
    }

    public class SessionUnitOfWork<P, D> : ISessionUnitOfWork<P, D>
    {

        private readonly Func<object, P[]> _getProxy;
        private readonly Func<P[], D[]> _getData;

        public SessionUnitOfWork(Func<object, P[]> getProxy, Func<P[], D[]> getData)
        {
            _getProxy = getProxy;
            _getData = getData;
        }

        // UNDONE remove api param

        public Dictionary<object, object> Execute(object filter)
        {
            var dict = new Dictionary<object, object>();
            try
            {
                var proxyList = RetryJob<P[]>.RunSingle(() => _getProxy(filter)).ToList();
                //var dataList = RetryJob<List<D>>.RunMany(proxyList, _getData, 1000);
                var dataList = ExecuteRemoveMissing(proxyList, 1000);

                for (int i = 0; i < proxyList.Count; ++i)
                {
                    var p = proxyList[i];
                    var d = dataList[i];
                    dict.Add(p, d);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Economic.Api.Exceptions.AuthorizationException"))
                    return dict;
                else
                    throw ex;
            }
            return dict;
        }

        public List<D> ExecuteRemoveMissing(List<P> proxyList, int maxBatchSize)
        {
            var result = new List<D>();
            int localBatchSize = proxyList.Count;
            if (localBatchSize > maxBatchSize)
            {
                foreach (var partialProxyList in proxyList.SpliceList(maxBatchSize))
                    result.AddRange(ExecuteRemoveMissing(partialProxyList, maxBatchSize));

                return result;
            }

            try
            {
                result = RetryJob<List<D>>.RunMany(proxyList, _getData, localBatchSize);
            }
            catch (Exception ex)
            {
                if (localBatchSize == 1)
                    return new List<D>();

                result.AddRange(ExecuteRemoveMissing(proxyList, (int)Math.Ceiling((double)(localBatchSize / 2))));

                //foreach (var partialProxyList in proxyList.SpliceList((int)Math.Ceiling((double)(localBatchSize / 2))))
                //    result.AddRange(ExecuteRemoveMissing(partialProxyList, maxBatchSize));
            }
            return result;
        }
    }
}
