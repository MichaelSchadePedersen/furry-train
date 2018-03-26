using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationMaker.BusinessLogic.Brokers.ERPBroker.EconomicBroker
{
    public class ExecuteObj<P,D>
    {
        public ExecuteObj(P[] proxyList,D[] datalist)
        {
            Datalist = datalist.ToList();
            Proxylist = proxyList.ToList();
        }
        public List<D> Datalist { get; set; }
        public List<P> Proxylist { get; set; }
    }
}
