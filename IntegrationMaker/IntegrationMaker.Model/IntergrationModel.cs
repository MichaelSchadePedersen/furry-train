using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationMaker.Model
{
    public class IntegrationModel
    {

        IntegrationCredentialModel _integrationCredentialModel;
        IntegrationTypeModel _integrationTypeModel;

        public long Id { get; set; }
        public long IntegrationTypeId { get; set; }
        public System.DateTime CreatedUTC { get; set; }
        public Nullable<System.DateTime> LastModifiedUTC { get; set; }
        public string Name { get; set; }
        public string ExternalNumber { get; set; }
        public long CompanyId { get; set; }
        public bool IsRunning { get; set; }
        public int ErrorCode { get; set; }
        public Nullable<System.DateTime> LastIncrementalDataImortUTC { get; set; }
        public Nullable<System.DateTime> LastFullDataImortUTC { get; set; }
        public IntegrationCredentialModel IntegrationCredentialModel
        { get => _integrationCredentialModel; set => _integrationCredentialModel = value; }
        public IntegrationTypeModel IntegrationTypeModel
        { get => _integrationTypeModel; set => _integrationTypeModel = value; }
    }
}