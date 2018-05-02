//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IntegrationMaker.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Integration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Integration()
        {
            this.IntegrationCredentials = new HashSet<IntegrationCredential>();
            this.IntegrationLogs = new HashSet<IntegrationLog>();
        }
    
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
    
        public virtual IntegrationType IntegrationType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntegrationCredential> IntegrationCredentials { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntegrationLog> IntegrationLogs { get; set; }
        public virtual Company Company { get; set; }
    }
}
