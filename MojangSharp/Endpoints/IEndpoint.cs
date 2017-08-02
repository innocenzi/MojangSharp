using MojangSharp.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MojangSharp
{
    public abstract class IEndpoint<T>
    {
        public Uri Address { get; set; }
        public Response Response { get; set; }
        public List<string> Arguments
        {
            get
            {
                if (_arguments == null)
                    _arguments = new List<string>() { };
                return _arguments;
            }
            set { _arguments = value; }
        }
        private List<string> _arguments;

        /// <summary>
        /// Contents for a post request, must be set before performing it.
        /// </summary>
        public string PostContent { get; set; }

        /// <summary>
        /// Performs the request
        /// </summary>
        /// <returns></returns>
        public abstract Task<T> PerformRequest();
    }
}
