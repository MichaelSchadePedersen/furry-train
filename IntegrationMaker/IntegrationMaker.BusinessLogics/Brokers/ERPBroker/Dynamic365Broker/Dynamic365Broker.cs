using IntegrationMaker.BusinessLogic.Brokers.ERPBroker.Dynamic365Broker.HelperClasses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.BusinessLogic.Brokers.ERPBroker.Dynamic365Broker
{
    public class Dynamic365Broker : ErpBroker
    {
        static HttpClient client = new HttpClient();
        public override Task PerformImportAsync(ImportType importType)
        {
            throw new NotImplementedException();
        }

        public override async Task VerifyIntegration()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
                    var byteArray = Encoding.ASCII.GetBytes("msp:AJ0Lu/zKdMzzWc6c5H/1mUpbbuPZsVNWuVkb+9ndLO4=");
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var response = httpClient.GetStringAsync(new Uri("https://api.businesscentral.dynamics.com/v1.0/capworks.eu/api/beta/companies")).Result;
                    var outer = Newtonsoft.Json.JsonConvert.DeserializeObject<ODataCompanyInformation>(response);    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
