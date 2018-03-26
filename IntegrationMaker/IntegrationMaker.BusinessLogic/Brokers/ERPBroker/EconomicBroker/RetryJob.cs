using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolpack.Shared.BusinessLogic;

namespace IntegrationMaker.BusinessLogic.Brokers.ERPBroker.EconomicBroker
{
    public class RetryLimitExceededException : Exception
    {
        public RetryLimitExceededException() : base()
        {

        }
        public RetryLimitExceededException(string message) : base(message)
        {

        }
        protected RetryLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
        public RetryLimitExceededException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }

    public class RetryJob<R>
    {
        private readonly int _maximumRetriesAllowed;
        private int _retriesAttempted;
        private readonly List<Func<R>> _jobs;
        private readonly List<R> _results;
        private readonly IList<Exception> _exceptions;
        private readonly Dictionary<Func<R>, object> _stateLookup;

        public event Action<Exception,object> ErrorSpecification;

        public RetryJob(int maximumRetriesAllowed)
        {
            _maximumRetriesAllowed = maximumRetriesAllowed;
            _jobs = new List<Func<R>>();
            _results = new List<R>();
            _exceptions = new List<Exception>();
            _stateLookup = new Dictionary<Func<R>, object>();
        }

        public void Load(Func<R> job)
        {
            _jobs.Add(job);
        }

        public void Load(Func<R> job, object state)
        {
            _jobs.Add(job);
            _stateLookup.Add(job, state);
        }

        private bool IsTransient(Exception exception)
        {
            if (exception.Message.Contains("Economic.Api.Exceptions.AuthorizationException"))
            {
                return false;
            }
            if (exception.Message.Contains("Economic.Api.Exceptions.ServerException"))
            {
                return false;
            }
            // TODO find solution
            //else if(exception is Economic.Api.Exceptions.AuthenticationException)
            //{
            //    return false;
            //}

            return true;
        }

        private bool RetryLimitExceeded(Task<R> task)
        {
            return _retriesAttempted > _maximumRetriesAllowed;                
        }

        public static List<R> RunMany<P, R>(List<P> proxy, Func<P[], R[]> job, int batchSize)
        {
            var proxyLists = proxy.SpliceList(batchSize);

            var retryJob = new RetryJob<List<R>>(10);
            foreach (var proxyList in proxyLists)
            {
                var localProxyList = proxyList.ToArray();
                retryJob.Load(() => job(localProxyList).ToList());
            }

            retryJob.Run();
            var dataObjectLists = retryJob.Result();
            return dataObjectLists.SelectMany(a => a).ToList();
        }

        public static R RunSingle<R>(Func<R> job)
        {
            var retryJob = new RetryJob<R>(10);
            retryJob.Load(job);
            retryJob.Run();
            return retryJob.Result().SingleOrDefault();
        }

        private R Try(Func<R> job)
        {
            var task = new Task<R>(job);

            task.RunSynchronously();

            if (task.IsFaulted)
            {
                var aggregateException = task.Exception.Flatten();

                foreach (var exception in aggregateException.InnerExceptions)
                {
                    _exceptions.Add(exception);
                }

                if (ErrorSpecification != null && _stateLookup.Any())
                    ErrorSpecification(aggregateException.InnerException, _stateLookup[job]);

                // Throws exception if non transient
                aggregateException.Handle(IsTransient);
                
                if (!RetryLimitExceeded(task))
                {    
                    // Incremental back off times
                    // Wait 0, 5, 10, 15,... seconds for each retry
                    Thread.Sleep(5000 * _retriesAttempted);
                    _retriesAttempted++;
                    return Try(job);
                   
                }
                else
                {
                    // Put in any transient exceptions causing the retry limit exceeding exception.
                    var innerException = new AggregateException(_exceptions);
                    throw new RetryLimitExceededException("Retry limit exceeded", innerException);
                }                                
            }

            return task.Result;
        }

        public void Run()
        {
            try
            {                
                foreach (var job in _jobs)
                {                 
                    _retriesAttempted = 0;
                    var result = Try(job);
                    _results.Add(result);
                }             
            }
            catch (AggregateException ex)
            {                
                throw ex.InnerException;
            }
        }

        public List<R> Result()
        {
            return _results;
        }
    }
}
