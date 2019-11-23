using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MojangSharp.Api
{
    /// <summary>
    /// Requester class, performs all the requests.
    /// </summary>
    public static class Requester
    {
        /// <summary>
        /// Defines timeout for http requests.
        /// </summary>
        public static TimeSpan Timeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Defines encoding for reading responses and writing requests.
        /// </summary>
        public static Encoding Encoding = Encoding.Default;

        /// <summary>
        /// Current name of Mojangsharp.
        /// </summary>
        public readonly static string Name = "MojangSharp";

        /// <summary>
        /// Current version of Mojangsharp.
        /// </summary>
        public readonly static string Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        /// <summary>
        /// An UUID representing this instance of requester. Change only if you know what it means.
        /// </summary>
        public static string ClientToken
        {
            get
            {
                if (_clientToken == null)
                    _clientToken = Guid.NewGuid().ToString();
                return _clientToken;
            }
            set { _clientToken = value; }
        }

        private static string _clientToken;

        /// <summary>
        /// Represents the http client used in the web requests.
        /// </summary>
        public static HttpClient Client
        {
            get
            {
                if (_client == null)
                    _client = new HttpClient() { Timeout = Requester.Timeout };
                return _client;
            }
            private set
            {
                _client = value;
            }
        }

        private static HttpClient _client;

        /// <summary>
        /// Sends a GET request to the given endpoint.
        /// </summary>
        /// <typeparam name="T">Type of the return response</typeparam>
        internal async static Task<Response> Get<T>(IEndpoint<T> endpoint, bool authenticate = false)
        {
            if (endpoint == null)
                throw new ArgumentNullException("Endpoint", "Endpoint should not be null.");

            HttpResponseMessage httpResponse = null;
            Error error = null;
            string rawMessage = null;

            try
            {
                // If there is a token to be given
                if (authenticate && endpoint.Arguments.Count > 0)
                {
                    //application/x-www-form-urlencoded
                    Requester.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", endpoint.Arguments[0]);
                    Requester.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    Requester.Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Requester.Name, Requester.Version));
                }

                httpResponse = await Requester.Client.GetAsync(endpoint.Address);
                rawMessage = await httpResponse.Content.ReadAsStringAsync();
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                error = new Error()
                {
                    ErrorMessage = ex.Message,
                    ErrorTag = ex.Message,
                    Exception = ex
                };
            }
            return new Response()
            {
                Code = httpResponse.StatusCode,
                RawMessage = rawMessage,
                IsSuccess = httpResponse.IsSuccessStatusCode && (
                            httpResponse.StatusCode == HttpStatusCode.Accepted ||
                            httpResponse.StatusCode == HttpStatusCode.Continue ||
                            httpResponse.StatusCode == HttpStatusCode.Created ||
                            httpResponse.StatusCode == HttpStatusCode.Found ||
                            httpResponse.StatusCode == HttpStatusCode.OK ||
                            httpResponse.StatusCode == HttpStatusCode.PartialContent ||
                            httpResponse.StatusCode == HttpStatusCode.NoContent) &&
                            error == null,
                Error = error
            };
        }

        /// <summary>
        /// Sends a POST request to the given endpoint.
        /// </summary>
        /// <typeparam name="T">Type of the return response</typeparam>
        internal async static Task<Response> Post<T>(IEndpoint<T> endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("Endpoint", "Endpoint should not be null.");

            if (endpoint.PostContent == null)
                throw new ArgumentNullException("PostContent", "PostContent should not be null.");

            HttpResponseMessage httpResponse = null;
            Error error = null;
            string rawMessage = null;

            try
            {
                StringContent contents = new StringContent(endpoint.PostContent, Requester.Encoding, "application/json");
                httpResponse = await Requester.Client.PostAsync(endpoint.Address, contents);
                rawMessage = await httpResponse.Content.ReadAsStringAsync();
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                error = new Error()
                {
                    ErrorMessage = ex.Message,
                    ErrorTag = ex.Message,
                    Exception = ex
                };
            }

            return new Response()
            {
                Code = httpResponse.StatusCode,
                RawMessage = rawMessage,
                IsSuccess = httpResponse.IsSuccessStatusCode && (
                            httpResponse.StatusCode == HttpStatusCode.Accepted ||
                            httpResponse.StatusCode == HttpStatusCode.Continue ||
                            httpResponse.StatusCode == HttpStatusCode.Created ||
                            httpResponse.StatusCode == HttpStatusCode.Found ||
                            httpResponse.StatusCode == HttpStatusCode.OK ||
                            httpResponse.StatusCode == HttpStatusCode.PartialContent) &&
                            error == null,
                Error = error
            };
        }

        /// <summary>
        /// Sends a POST request to the given endpoint.
        /// </summary>
        internal async static Task<Response> Post<T>(IEndpoint<T> endpoint, Dictionary<string, string> toEncode)
        {
            if (endpoint == null)
                throw new ArgumentNullException("Endpoint", "Endpoint should not be null.");

            if (toEncode == null || toEncode.Count < 1)
                throw new ArgumentNullException("PostContent", "PostContent should not be null.");

            HttpResponseMessage httpResponse = null;
            Error error = null;
            string rawMessage = null;

            try
            {
                //application/x-www-form-urlencoded
                Requester.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", endpoint.Arguments[0]);
                Requester.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                Requester.Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MojangSharp", "0.1"));

                httpResponse = await Requester.Client.PostAsync(endpoint.Address, new FormUrlEncodedContent(toEncode));
                rawMessage = await httpResponse.Content.ReadAsStringAsync();
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized ||
                    httpResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    JObject err = JObject.Parse(rawMessage);
                    error = new Error()
                    {
                        ErrorMessage = err["errorMessage"].ToObject<string>(),
                        ErrorTag = err["error"].ToObject<string>(),
                        Exception = ex
                    };
                }
                else
                {
                    error = new Error()
                    {
                        ErrorMessage = ex.Message,
                        ErrorTag = ex.GetBaseException().Message,
                        Exception = ex
                    };
                }
            }

            return new Response()
            {
                Code = httpResponse.StatusCode,
                RawMessage = rawMessage,
                IsSuccess = httpResponse.IsSuccessStatusCode && (
                            httpResponse.StatusCode == HttpStatusCode.Accepted ||
                            httpResponse.StatusCode == HttpStatusCode.Continue ||
                            httpResponse.StatusCode == HttpStatusCode.Created ||
                            httpResponse.StatusCode == HttpStatusCode.Found ||
                            httpResponse.StatusCode == HttpStatusCode.OK ||
                            httpResponse.StatusCode == HttpStatusCode.PartialContent) &&
                            error == null,
                Error = error
            };
        }

        /// <summary>
        /// Sends a PUT request to the given endpoint.
        /// </summary>
        internal async static Task<Response> Put<T>(IEndpoint<T> endpoint, Uri file)
        {
            if (endpoint == null)
                throw new ArgumentNullException("Endpoint", "Endpoint should not be null.");

            
            if (file == null || string.IsNullOrEmpty(file.ToString()))
                throw new ArgumentNullException("Skin", "No file given.");

            HttpResponseMessage httpResponse = null;
            Error error = null;
            string rawMessage = null;

            try
            {
                //application/x-www-form-urlencoded
                Requester.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", endpoint.Arguments[0]);
                Requester.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                Requester.Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MojangSharp", "0.1"));

                using (var contents = new MultipartFormDataContent())
                {
                    contents.Add(new StringContent(bool.Parse(endpoint.Arguments[1]) == true ? "true" : "false"), "model");
                    using (WebClient wc = new WebClient()) {
                        contents.Add(new ByteArrayContent(wc.DownloadData(file)), "file", Path.GetFileName(file.ToString()));
                    }

                    httpResponse = await Requester.Client.PutAsync(endpoint.Address, contents);
                    rawMessage = await httpResponse.Content.ReadAsStringAsync();
                    httpResponse.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized ||
                    httpResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    JObject err = JObject.Parse(rawMessage);
                    error = new Error()
                    {
                        ErrorMessage = err["errorMessage"].ToObject<string>(),
                        ErrorTag = err["error"].ToObject<string>(),
                        Exception = ex
                    };
                }
                else
                {
                    error = new Error()
                    {
                        ErrorMessage = ex.Message,
                        ErrorTag = ex.GetBaseException().Message,
                        Exception = ex
                    };
                }
            }

            return new Response()
            {
                Code = httpResponse.StatusCode,
                RawMessage = rawMessage,
                IsSuccess = httpResponse.IsSuccessStatusCode && (
                            httpResponse.StatusCode == HttpStatusCode.Accepted ||
                            httpResponse.StatusCode == HttpStatusCode.Continue ||
                            httpResponse.StatusCode == HttpStatusCode.Created ||
                            httpResponse.StatusCode == HttpStatusCode.Found ||
                            httpResponse.StatusCode == HttpStatusCode.OK ||
                            httpResponse.StatusCode == HttpStatusCode.PartialContent) &&
                            error == null,
                Error = error
            };
        }

        /// <summary>
        /// Sends a DELETE request to the given endpoint.
        /// </summary>
        internal async static Task<Response> Delete<T>(IEndpoint<T> endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("Endpoint", "Endpoint should not be null.");

            HttpResponseMessage httpResponse = null;
            Error error = null;
            string rawMessage = null;

            try
            {
                //application/x-www-form-urlencoded
                Requester.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", endpoint.Arguments[0]);
                Requester.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                Requester.Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MojangSharp", "0.1"));

                httpResponse = await Requester.Client.DeleteAsync(endpoint.Address);
                rawMessage = await httpResponse.Content.ReadAsStringAsync();
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized ||
                    httpResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    JObject err = JObject.Parse(rawMessage);
                    error = new Error()
                    {
                        ErrorMessage = err["errorMessage"].ToObject<string>(),
                        ErrorTag = err["error"].ToObject<string>(),
                        Exception = ex
                    };
                }
                else
                {
                    error = new Error()
                    {
                        ErrorMessage = ex.Message,
                        ErrorTag = ex.GetBaseException().Message,
                        Exception = ex
                    };
                }
            }

            return new Response()
            {
                Code = httpResponse.StatusCode,
                RawMessage = rawMessage,
                IsSuccess = httpResponse.IsSuccessStatusCode && (
                            httpResponse.StatusCode == HttpStatusCode.Accepted ||
                            httpResponse.StatusCode == HttpStatusCode.Continue ||
                            httpResponse.StatusCode == HttpStatusCode.Created ||
                            httpResponse.StatusCode == HttpStatusCode.Found ||
                            httpResponse.StatusCode == HttpStatusCode.OK ||
                            httpResponse.StatusCode == HttpStatusCode.PartialContent) &&
                            error == null,
                Error = error
            };
        }
    }
}