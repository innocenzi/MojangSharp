using System;
using System.Net;

namespace MojangSharp
{
    /// <summary>
    /// Default response class, can be inherited.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Status code of the response.
        /// </summary>
        public HttpStatusCode Code { get; internal set; }

        /// <summary>
        /// Defines weither or note the request is a success.
        /// </summary>
        public bool IsSuccess { get; internal set; }

        /// <summary>
        /// Response's raw message contents.
        /// </summary>
        public string RawMessage { get; internal set; }

        /// <summary>
        /// Contains an error if the request failed.
        /// </summary>
        public Error Error { get; internal set; }

        internal Response()
        {
        }

        internal Response(Response response) : this()
        {
            this.Code = response.Code;
            this.IsSuccess = response.IsSuccess;
            this.RawMessage = response.RawMessage;
            this.Error = response.Error;
        }
    }

    /// <summary>
    /// Default error class.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Tag of the given error.
        /// </summary>
        public string ErrorTag { get; internal set; }

        /// <summary>
        /// Details of the error.
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// Exception if code-side error.
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// Gets an error thanks to a response.
        /// </summary>
        public static Response GetError(Response response)
        {
            // This has to be fill
            switch (response.Code)
            {
                case HttpStatusCode.NoContent:
                    {
                        return new Response(response)
                        {
                            IsSuccess = false,
                            Error = new Error()
                            {
                                ErrorMessage = "Response has no content",
                                ErrorTag = "NoContent"
                            }
                        };
                    }

                case HttpStatusCode.UnsupportedMediaType:
                    {
                        return new Response(response)
                        {
                            IsSuccess = false,
                            Error = new Error()
                            {
                                ErrorMessage = "Post contents must not be well formatted",
                                ErrorTag = "UnsupportedMediaType"
                            }
                        };
                    }

                default:
                    {
                        return new Response(response)
                        {
                            IsSuccess = false,
                            Error = new Error()
                            {
                                ErrorMessage = response.Code.ToString(),
                                ErrorTag = response.Code.ToString()
                            }
                        };
                    }
            }
        }
    }
}