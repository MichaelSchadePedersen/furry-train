using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.BusinessLogic.TaskConfiguration
{
    public class ProvisioningResult
    {
        public Boolean? OK { get; set; } //Null is used for postponing.
        public String Message { get; set; }

        public ProvisioningResult()
        {
            OK = false;
        }

        public override string ToString()
        {
            if (OK == true)
                return "OK: " + Message;
            else if (OK == null)
                return "Tentative: " + Message;
            else
                return "Error: " + Message;
        }
    }
}
