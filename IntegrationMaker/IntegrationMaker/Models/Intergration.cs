using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationMaker.Models
{
    public class Integration
    {
        string _name;
        Int64 _ExternalNumber;
        public string Name { get => _name; set => _name = value; }
        public Int64 ExternalNumber { get => _ExternalNumber; set => _ExternalNumber = value; }
    }
}