using System;

namespace IntegrationMaker.Models
{
    public class IntegrationModel
    {
        string _name;
        Int64 _ExternalNumber;
        public string Name { get => _name; set => _name = value; }
        public Int64 ExternalNumber { get => _ExternalNumber; set => _ExternalNumber = value; }
    }
}