using System;
using System.Collections.Generic;
using System.Linq;



namespace IntegrationMaker.BusinessLogic.Brokers.ERPBroker.EconomicBroker
{
    /// <summary>
    /// Class used to access the economic api in a uniform and generic way.
    /// </summary>
    public class EconomicApiAccess
    {
        
        public Dictionary<Type, Dictionary<object, object>> TypeLookup { get; private set; }
        public Dictionary<Type, IList<object>> TypeListLookup { get; private set; }

        public EconomicApiAccess()
        {
           
            TypeLookup = new Dictionary<Type, Dictionary<object, object>>();
            TypeListLookup = new Dictionary<Type, IList<object>>();

        }

        #region Public methods

        /// <summary>
        /// Returns a dictionary used to cache and late lookup values.
        /// </summary>
        public Dictionary<P, D> BuildDictionary<P, D>(Func<P[]> getProxy, Func<P[], D[]> getData)
        {
          

            var dict = new Dictionary<P, D>();
            var genericDict = new Dictionary<object, object>();
            TypeLookup.Add(typeof(P), genericDict);

            try
            {
                var tuple = GetData(getProxy, getData);
                var proxies = tuple.Item1;
                var datas = tuple.Item2;

                for (int i = 0; i < proxies.Count; ++i)
                {
                    var p = proxies[i];
                    var d = datas[i];
                    dict.Add(p, d);
                    genericDict.Add(p, d);
                }
            }

            //catch (FaultException ex)
            //{

            //}
            catch (Exception ex)
            {
              
            }
            return dict;
        }

        public Dictionary<P, D> GetUpdate<P, D>(Func<DateTime, bool, P[]> getProxy, Func<P[], D[]> getData, DateTime updateDate)
        {
            return BuildDictionary(() => getProxy(updateDate, true), getData);
        }

        #endregion

        #region Private methods
        
        private Tuple<IList<P>, IList<D>> GetData<P, D>(Func<P[]> getProxy, Func<P[], D[]> getData)
        {
            var proxies = RetryJob<List<P>>.RunSingle(() => getProxy().ToList());
            var datas = RetryJob<D>.RunMany(proxies, getData, 1000);

            return new Tuple<IList<P>, IList<D>>(proxies, datas);
        }

        private IList<D> BuildList<P, D>(Func<P[]> getProxy, Func<P[], D[]> getData)
        {
            var tuple = GetData(getProxy, getData);
            TypeListLookup.Add(typeof(P), tuple.Item2.Cast<object>().ToList());
            return tuple.Item2;
        }

        #endregion
    }
}
