using EconomicSoapService;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
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

        public override async Task VerifyIntegration()
        {
            try
            {
                soapSession = getSoapSession();
                //await GetCompanyAsync();
            }
            catch (Exception ex)
            {
                throw ex; //ErpIntegrationException(ErpIntegrationErrorCodes.InvalidCredentials, "Economic");
            }
        }

        private EconomicWebServiceSoapClient getSoapSession()
        {


            EconomicWebServiceSoapClient client = new EconomicWebServiceSoapClient();
            ((BasicHttpBinding)client.Endpoint.Binding).AllowCookies = true;
            using (var opretationsScope = new OperationContextScope(client.InnerChannel))
            {


                // Add a HTTP Header to an outgoing request
                var requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers["X-EconomicAppIdentifier"] = "FurryTrain/1.1 (https://www.furrytrain.dk; dev@furrytrain.dk) .NET Service Reference ";
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

                switch (Integration.IntegrationTypeId)
                {
                    case 1: //Normal customer-agreement                    
                        var loginDetails = Credential;
                        if (!string.IsNullOrEmpty(loginDetails.Token))
                        {
                            client.ConnectWithToken(loginDetails.Token, "BziA4Tls2Bgp4STB3lDJH0ReTtoUwK8xMCHC1AXMwI81");
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
