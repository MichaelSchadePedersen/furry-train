using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationMaker.Model
{
    public class LeadModel
    {
        public long Id { get; set; }
        public string Company { get; set; }
        public string LastName { get; set; }
        public string EMail { get; set; }
    }
}