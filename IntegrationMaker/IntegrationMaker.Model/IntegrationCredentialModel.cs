using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationMaker.Model
{
    public class IntegrationCredentialModel
    {
        
        public long Id { get; set; }
        public string AccountIdentifier { get; set; }
        public string Username { get; set; }
        public long IntegrationId { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public System.DateTime CratedUTC { get; set; }
        public Nullable<System.DateTime> LastModifiedUTC { get; set; }
    }
}