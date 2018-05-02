using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.BusinessLogic.Brokers.ERPBroker.Dynamic365Broker.HelperClasses
{
   public class ODataCompanyInformation
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }
        public List<CompanyInformationValues> Value { get; set; }
    }
}
