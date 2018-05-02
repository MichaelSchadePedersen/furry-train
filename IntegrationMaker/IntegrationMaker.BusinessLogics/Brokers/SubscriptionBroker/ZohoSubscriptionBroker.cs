using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using EasyHttp.Http;
using IntegrationMaker.Model;

namespace IntegrationMaker.BusinessLogic.Brokers.SubscriptionBroker
{
    public class ZohoSubscriptionBroker
    {
        static string ZohoApiBaseUrl = "https://subscription.zoho.com/crm/private/xml/Leads/insertRecords";
        static string ZohoApiKey = "02ec5f3ba870a12a668b993ebee5ec48";

        public bool CreateLead(LeadModel leadModel)
        {

            XDocument xmlData = new XDocument(
            new XElement("Leads",
                new XElement("row", new XAttribute("no", "1"),
                    new XElement("FL", new XAttribute("val", "Lead Source"), "Trial Signup"),
                    new XElement("FL", new XAttribute("val", "Company"), leadModel.Company),
                    new XElement("FL", new XAttribute("val", "Last Name"), leadModel.LastName),
                    new XElement("FL", new XAttribute("val", "Email"), leadModel.EMail)
                )));

            string url = string.Format("{0}?authtoken={1}&scope=crmapi&newFormat=1&xmlData={2}",
                ZohoApiBaseUrl, ZohoApiKey,
                HttpUtility.UrlEncode(xmlData.ToString()));

            var http = new EasyHttp.Http.HttpClient
            {
                Request = { Accept = HttpContentTypes.ApplicationXml }
            };

            dynamic emptyPost = new ExpandoObject();
            EasyHttp.Http.HttpResponse response;
            try
            {
                response = http.Post(url, emptyPost, HttpContentTypes.ApplicationXml);
            }
            catch (WebException ex)
            {

                return false;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            XDocument xdoc;
            try
            {
                xdoc = XDocument.Parse(response.RawText);
            }
            catch (XmlException ex)
            {
                return false;
            }

            string msg;
            try
            {
                msg = xdoc.Descendants("result").First().Element("message").Value;
            }
            catch (Exception ex)
            {
                return false;
            }
            if (msg != "Record(s) added successfully")
            {
                return false;
            }

            return true;
        }
    }
}