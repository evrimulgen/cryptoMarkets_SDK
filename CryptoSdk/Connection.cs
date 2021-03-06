﻿using CryptoSdk.Misc;
using DomainModel.Features;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSdk
{
    public abstract class Connection : IConnection
    {
        protected abstract string MainUri { get; }

        public T PublicGetQuery<T>(string endPoint)
        {
            return ((IConnection) this).PublicGetQuery<T>(endPoint, default(Tuple<string, string>[]));
        }

        public T PublicGetQuery<T>(string endPoint, Tuple<string, string> parameter)
        {
            var parameters = new Tuple<string, string>[1];
            parameters[0] = parameter;
            return ((IConnection)this).PublicGetQuery<T>(endPoint, parameters);
        }

        public T PublicGetQuery<T>(string endPoint, Tuple<string, string>[] parameters)
        {
            var uri = $"{MainUri}{endPoint}";

            return CallGetRequestWithJsonResponse<T>(uri, parameters);
        }

        /*public T PublicGetQuery<T>(string endPoint, Tuple<string, string>[] parameters, Tuple<string, string>[] headers)
        {
            var uri = $"{MainUri}{endPoint}";

            return CallGetRequestWithJsonResponse<T>(uri, parameters, headers);
        }*/

        public abstract T PrivateGetQuery<T>(string endPoint, Authenticator secretKey, Tuple<string, string>[] parameters);

        public abstract T PrivatePostQuery<T>(string endPoint, Authenticator keys, Tuple<string, string>[] parameters);

        protected static string HashHmac(string message, string secretKey)
        {
            var encoding = Encoding.UTF8;
            using (var hmac = new HMACSHA512(encoding.GetBytes(secretKey)))
            {
                var msg = encoding.GetBytes(message);
                var hash = hmac.ComputeHash(msg);
                return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
            }
        }

        protected string CodeGetParams(IReadOnlyCollection<Tuple<string, string>> parameters)
        {
            if (parameters == null || parameters.Count <= 0)
                return null;

            var extraParameters = new StringBuilder();
            foreach (var item in parameters)
                extraParameters.Append((extraParameters.Length == 0 ? "?" : "&") + item.Item1 + "=" + item.Item2);

            return extraParameters.Length > 0 ? extraParameters.ToString() : null;
        }

        protected string CodePostParams(IReadOnlyCollection<Tuple<string, string>> parameters)
        {
            if (parameters == null || parameters.Count <= 0)
                return null;

            var extraParameters = new StringBuilder();
            foreach (var item in parameters)
                extraParameters.Append((extraParameters.Length == 0 ? "" : "&") + item.Item1 + "=" + item.Item2);

            return extraParameters.Length > 0 ? extraParameters.ToString() : null;
        }

        private readonly TimeSpan _mDefaultTimeOut = new TimeSpan(TimeSpan.TicksPerMinute * 30); // Default timeout - 30 minutes

        private HttpClient CreateHttpClient()
        {
            var result = new HttpClient
            {
                Timeout = _mDefaultTimeOut
            };

            result.DefaultRequestHeaders.Accept.Clear();
            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return result;
        }

        protected T CallGetRequestWithJsonResponse<T>(string uri,
            IReadOnlyCollection<Tuple<string, string>> parameters, params Tuple<string, string>[] headers)
        {
            var result = default(T);

            using (var httpClient = CreateHttpClient())
            {
                var request = MakeGetRequest(uri, parameters, headers);

                var response = httpClient.SendAsync(request);
                response.Wait();
                if (response.IsCompleted)
                {
                    try
                    {
                        result = response.Result.Content.ReadAsStreamAsync().Result.ReadObject<T>();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                return result;
            }
        }

        private HttpRequestMessage MakeGetRequest(string uri,
            IReadOnlyCollection<Tuple<string, string>> parameters, Tuple<string, string>[] headers)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{uri}{CodeGetParams(parameters)}"),
                Method = HttpMethod.Get,
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Item1, header.Item2);
                }
            }

            return request;
        }

        protected T CallPostRequestWithJsonResponse<T>(string uri,
            IReadOnlyCollection<Tuple<string, string>> parameters, params Tuple<string, string>[] headers)
        {
            var result = default(T);

            using (var httpClient = CreateHttpClient())
            {
                var request = MakePostRequest(uri, parameters, headers);

                var response = httpClient.SendAsync(request);
                response.Wait();
                if (response.IsCompleted)
                {
                    result = response.Result.Content.ReadAsStreamAsync().Result.ReadObject<T>();
                }
                return result;
            }
        }

        private HttpRequestMessage MakePostRequest(string uri,
            IReadOnlyCollection<Tuple<string, string>> parameters, Tuple<string, string>[] headers)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(uri),
                Method = HttpMethod.Post,
                Content = new StringContent(CodePostParams(parameters), Encoding.Default, "application/x-www-form-urlencoded"),
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Item1, header.Item2);
                }
            }

            return request;
        }
    }
}