using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationMaker.Model
{
    public class IntegrationTypeModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public System.DateTime CreatedUTC { get; set; }
    }
}