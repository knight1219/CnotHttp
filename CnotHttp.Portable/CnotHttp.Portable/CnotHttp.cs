using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;

namespace CnotHttp.Portable
{
    public class CnotHttp
    {
        public static async Task<List<ICnotResult>> RunCnots(List<Task<ICnotResult>> cnots)
        {
            var results = await Task.WhenAll(cnots);
            return results.ToList();
        }

        public static async Task<ICnotResult<T>> Get<T>(string url, List<KeyValuePair<string, string>> headers)
        {
            return await CreateHttpCall<T>("GET", url, headers, null);
        }

        public static async Task<ICnotResult<string>> Get(string url, List<KeyValuePair<string, string>> headers)
        {
            return await CreateHttpCall<string>("GET", url, headers, null);
        }

        public static async Task<ICnotResult<T>> Get<T>(string url)
        {
            return await CreateHttpCall<T>("GET", url, null, null);
        }

        public static async Task<ICnotResult<string>> Get(string url)
        {
            return await CreateHttpCall<string>("GET", url, null, null);
        }

        public static async Task<ICnotResult<T>> Put<T>(string url, List<KeyValuePair<string, string>> headers, object content)
        {
            return await CreateHttpCall<T>("PUT", url, headers, content);
        }

        public static async Task<ICnotResult<string>> Put(string url, List<KeyValuePair<string, string>> headers, object content)
        {
            return await CreateHttpCall<string>("PUT", url, headers, content);
        }

        public static async Task<ICnotResult<T>> Put<T>(string url, object content)
        {
            return await CreateHttpCall<T>("PUT", url, null, content);
        }

        public static async Task<ICnotResult<string>> Put(string url, object content)
        {
            return await CreateHttpCall<string>("PUT", url, null, content);
        }

        public static async Task<ICnotResult<T>> Post<T>(string url, List<KeyValuePair<string, string>> headers, object content)
        {
            return await CreateHttpCall<T>("POST", url, headers, content);
        }

        public static async Task<ICnotResult<string>> Post(string url, List<KeyValuePair<string, string>> headers, object content)
        {
            return await CreateHttpCall<string>("POST", url, headers, content);
        }

        public static async Task<ICnotResult<T>> Post<T>(string url, object content)
        {
            return await CreateHttpCall<T>("POST", url, null, content);
        }

        public static async Task<ICnotResult<string>> Post(string url, object content)
        {
            return await CreateHttpCall<string>("POST", url, null, content);
        }

        public static async Task<ICnotResult<T>> Delete<T>(string url, List<KeyValuePair<string, string>> headers)
        {
            return await CreateHttpCall<T>("POST", url, headers, null);
        }

        public static async Task<ICnotResult<string>> Delete(string url, List<KeyValuePair<string, string>> headers)
        {
            return await CreateHttpCall<string>("POST", url, headers, null);
        }

        public static async Task<ICnotResult<T>> Delete<T>(string url)
        {
            return await CreateHttpCall<T>("POST", url, null, null);
        }

        public static async Task<ICnotResult<string>> Delete(string url)
        {
            return await CreateHttpCall<string>("POST", url, null, null);
        }


        private static HttpClient CreateClient(string url, List<KeyValuePair<string, string>> headers)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            
            if ( headers != null)
            {
                foreach (var item in headers)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            return client;
        }

        private static async Task<CnotResult<T>> CreateHttpCall<T>(string method, string url, List<KeyValuePair<string, string>> headers, object data)
        {
            var cnotResult = new CnotResult<T>();
            using (var client = CreateClient(url, headers))
            {
                Task<HttpResponseMessage> clientTask = null;
                switch (method)
                {
                    case "GET":
                        clientTask = client.GetAsync("");
                        break;
                    case "PUT":
                        clientTask = client.PutAsync("", new StringContent(JsonConvert.SerializeObject(data)));
                        break;
                    case "POST":
                        clientTask = client.PostAsync("", new StringContent(JsonConvert.SerializeObject(data)));
                        break;
                    case "DELETE":
                        clientTask = client.DeleteAsync("");
                        break;
                    default:
                        cnotResult.ServerResponse = default(T);
                        cnotResult.Error = new InvalidOperationException("Invalid Http method detected");
                        break;
                }


                if (clientTask != null)
                {
                    try
                    {
                        var httpTask = await ProcessHttpCall(clientTask);
                        if (httpTask.Outcome == OutcomeType.Successful)
                        {
                            var stringResult = await httpTask.Result.Content.ReadAsStringAsync();


                            if (typeof(T) == typeof(string))
                            {
                                cnotResult.ServerResponse = (T)Convert.ChangeType(stringResult, typeof(T));
                            }
                            else
                            {
                                cnotResult.ServerResponse = JsonConvert.DeserializeObject<T>(stringResult);
                            }

                            cnotResult.Error = null;
                        }
                        else
                        {
                            cnotResult.ServerResponse = default(T);
                            cnotResult.Error = new Exception(
                                "Unable to process http request. Status Code " + (int)httpTask.FinalHandledResult.StatusCode,
                                httpTask.FinalException);
                        }
                    }
                    catch (JsonException e)
                    {
                        cnotResult.ServerResponse = default(T);
                        cnotResult.Error = new ArgumentException("Unable to convert response into object", e);
                    }
                    catch (Exception e)
                    {
                        cnotResult.ServerResponse = default(T);
                        cnotResult.Error = new InvalidOperationException("Unable to complete request", e);
                    }
                    
                }  
            }

            return cnotResult;
        }

        private static async Task<PolicyResult<HttpResponseMessage>> ProcessHttpCall(Task<HttpResponseMessage> task)
        {
            var httpStatusCodesWorthRetrying = (new int[] { 408, 500, 502, 503, 504 }).ToList();

            var exceptionPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<WebException>()
                .Or<InvalidOperationException>()
                .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains((int)r.StatusCode))
                .OrResult(r => r.IsSuccessStatusCode == false)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            PolicyResult<HttpResponseMessage> response = await exceptionPolicy.ExecuteAndCaptureAsync(async () => await task); ;

            return response;
        }
    }
}