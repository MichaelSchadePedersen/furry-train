using EconomicSoapService;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.BusinessLogic.Brokers.ERPBroker.EconomicBroker
{
    public class EconomicBroker : ErpBroker
    {
        private EconomicWebServiceSoapClient soapSession = null;
        private LookupLists lookups;
        private string integrationName = string.Empty;

        public override Task PerformImportAsync(ImportType importType)
        {
            throw new NotImplementedException();
        }

        public override Task VerifyIntegration()
        {
            throw new NotImplementedException();
        }

        private EconomicWebServiceSoapClient getSoapSession()
        {


            EconomicWebServiceSoapClient client = new EconomicWebServiceSoapClient();
            ((BasicHttpBinding)client.Endpoint.Binding).AllowCookies = true;
            using (var opretationsScope = new OperationContextScope(client.InnerChannel))
            {


                // Add a HTTP Header to an outgoing request
                var requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers["X-EconomicAppIdentifier"] = "Toolpack Solutions/1.1 (http://toolpack.net/loesninger/toolpack-online/; dev@toolpack.net) .NET Service Reference ";
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

                switch (Integration.IntegrationTypeId)
                {
                    case 2: //Normal customer-agreement                    
                        var loginDetails = Credential;
                        if (!string.IsNullOrEmpty(loginDetails.Token))
                        {
                            client.ConnectWithToken(loginDetails.Token, "_7Wtbg4jmo_IZQDrZxr9E54pH9JiSP43ayY-udeM41s1");
                            Integration.Name = client.Company_GetName(client.Company_Get());
                            //Integration.Number = int.Parse(client.Company_GetNumber(client.Company_Get()));
                        }
                        else
                        {
                            client.Connect(int.Parse(Integration.ExternalNumber), loginDetails.Username, loginDetails.Password);
                            Integration.Name = client.Company_GetName(client.Company_Get());
                            //Integration.Number = int.Parse(client.Company_GetNumber(client.Company_Get()));
                        }
                        break;
                    case 3: //Administrator-agreement
                        var adminCredentials = Credential;
                        client.ConnectAsAdministrator(int.Parse(adminCredentials.AccountIdentifier), adminCredentials.Username,
                                                       adminCredentials.Password, int.Parse(Integration.ExternalNumber));
                        Integration.Name = client.Company_GetName(client.Company_Get());
                        //Integration.Number = int.Parse(client.Company_GetNumber(client.Company_Get()));

                        break;
                    default:
                        break;
                }

                return client;
            }

        }
    }
}
