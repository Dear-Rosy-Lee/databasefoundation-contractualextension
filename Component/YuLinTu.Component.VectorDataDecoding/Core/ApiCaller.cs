using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Navigation;
using static YuLinTu.tGISCNet.PointAreaRelationCheck;

namespace YuLinTu.Component.VectorDataDecoding
{
    public class ApiCaller
    {
        private const int PollingInterval = 5000; // 轮询间隔时间（毫秒）
        private const int Timeout = 1800000; // 超时时间（毫秒）


        public HttpClient client { get; set; }

        public string ErrorInfo { get; set; }

        public string PostGetTaskIDAsync(string token, string url, string jsonData)
        {
            try
            {
                // 将JSON数据转换为HttpContent
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                content.Headers.Add("token", token);
                // 发送 POST 请求
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                // 读取响应内容
                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                //
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    return root.GetProperty("data").GetString();
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public string PostDataAsync(string url, string jsonData, string token = "", string szdy = "")
        {
            try
            {
                // 将JSON数据转换为HttpContent
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Clear();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("token", token);
                }
                if (!string.IsNullOrEmpty(szdy))
                {
                    client.DefaultRequestHeaders.Add("szdy", szdy);
                }
                // 发送 POST 请求
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                // 读取响应内容
                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement data = new JsonElement();
                        var res = root.TryGetProperty("result", out data);
                        if (res)
                        {
                            data = root.GetProperty("result");
                            JsonElement jsuccess = root.GetProperty("success");
                            if (jsuccess.ToString().ToLower().Equals("false"))
                            {
                                string err = root.GetProperty("errormsg").ToString();
                                throw new Exception(err);
                            }
                            else
                            {
                                string points = data.GetProperty("points").ToString();
                                return points;
                            }
                        }

                        var updata = root.TryGetProperty("data", out data);
                        if (updata)
                        {

                            JsonElement jsuccess = root.GetProperty("data");
                            return jsuccess.ToString();
                        }
                    }
                }
                return "";
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
        }
        public string PostDataAsync(string url, Dictionary<string, string> Headers, string jsonData,out bool sucess)
        {
            var responseBody=string.Empty;
            try
            {
                sucess = false;
                // 将JSON数据转换为HttpContent
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Clear();
                foreach (var kvp in Headers)
                {
                    client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
               
                // 发送 POST 请求
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                // 读取响应内容
                responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();                  
                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement data = root.GetProperty("data");  //new JsonElement();
                        //var res = root.GetProperty("data"); //root.TryGetProperty("data", out data);

                        //var msg = root.TryGetProperty("message", out data);

                        sucess = true;
                       return data.ToString();
               
                    }
                }
                return "";
            }
            catch (HttpRequestException ex)
            {
                sucess = false;
                var msg = responseBody.ToString();         
                return msg;
            }
        }

        //public class ResponseEntity
        //{
        //    public string data { get; set; }
        //    public string Message { get; set; }
        //    public string Errors { get; set; }
        //}
        public  T PostResultAsync<T>(string url, Dictionary<string, string> Headers, string jsonData) where T : class, new()
        {
          
            try
            {
                // 将JSON数据转换为HttpContent
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Clear();
                foreach (var kvp in Headers)
                {
                    client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }

                // 发送 POST 请求
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                // 读取响应内容
                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement data = root.GetProperty("data");

                        // 提取并解析 results 字段的内容
                        string resultsJson = data.ToString();
                     
                        var entity = JsonSerializer.Deserialize<T>(resultsJson);
                        // 再次解析 results 字符串为 JSON 对象
                        return entity;
                      
                    }
                }
                else
                {
                    return null;
                }
             
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }

        }
        public List<T> PostResultListAsync<T>(string url, Dictionary<string, string> Headers, string jsonData) where T : class, new()
        {

            try
            {
                // 将JSON数据转换为HttpContent
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Clear();
                foreach (var kvp in Headers)
                {
                    client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }

                // 发送 POST 请求
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();

        

                // 读取响应内容
                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement data = root.GetProperty("data");

                        // 提取并解析 results 字段的内容
                        string resultsJson = data.ToString();

                        var entity = JsonSerializer.Deserialize<List<T>>(resultsJson);
                        // 再次解析 results 字符串为 JSON 对象
                        return entity;

                    }
                }
                else
                {
                    return null;
                }

            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }

        }
        public List<T> PostResultListAsync2<T>(string url, Dictionary<string, string> Headers, string jsonData) where T : class, new()
        {

            try
            {
                // 将JSON数据转换为HttpContent
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Clear();
                foreach (var kvp in Headers)
                {
                    client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }

                // 发送 POST 请求
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                // 读取响应内容
                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement data = root.GetProperty("data");

                        // 提取并解析 results 字段的内容
                        string resultsJson = data.ToString();

                        var entity = JsonSerializer.Deserialize<List<T>>(resultsJson);
                        // 再次解析 results 字符串为 JSON 对象
                        return entity;

                    }
                }
                else
                {
                    return null;
                }

            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }

        }

        public KeyValueList<string, string> PostGetResultAsync(string token, string url, string taskID)
        {
            var cancellationTokenSource = new CancellationTokenSource(Timeout); // 设置超时
            var cancellationToken = cancellationTokenSource.Token;
            string responseBody;
            try
            {
                while (true)
                {
                    string url1 = $"{url}?jobId={taskID}";
                    client.DefaultRequestHeaders.Add("accept", "*/*");
                    client.DefaultRequestHeaders.Add("token", token);
                    // 发送 Get 请求
                    HttpResponseMessage response = client.GetAsync(url1).ConfigureAwait(false).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    // 读取响应内容
                    responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        using (JsonDocument doc = JsonDocument.Parse(responseBody))
                        {
                            JsonElement root = doc.RootElement;
                            JsonElement data = root.GetProperty("data");
                            if (data.ValueKind != JsonValueKind.Null)
                            {
                                string resultsJson = data.GetProperty("results").GetString();
                                if (!resultsJson.IsNullOrEmpty())
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ErrorInfo = $"查询任务时出错： {response.StatusCode}";
                        break;
                    }

                    // 延迟轮询
                    Task<KeyValueList<string, string>>.Delay(PollingInterval, cancellationToken);
                    url1 = string.Empty;
                    client.DefaultRequestHeaders.Clear();
                }
                return GetData(responseBody);
            }
            catch (TaskCanceledException)
            {
                ErrorInfo = "轮询超时，任务未完成。";
            }
            catch (Exception ex)
            {
                ErrorInfo = "出现异常：" + ex.Message;
            }
            return null;
        }

        public T GetResultAsync<T>(string url, Dictionary<string,string> Headers) where T:class,new()
        {
            var en = new T();
            client.DefaultRequestHeaders.Clear();
            foreach (var kvp in Headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }
            try
            {
              HttpResponseMessage response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
              response.EnsureSuccessStatusCode();
               string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
             // 读取响应内容
              responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement data = root.GetProperty("data");

                    // 提取并解析 results 字段的内容
                    string resultsJson = data.ToString();
               
                    var entity = JsonSerializer.Deserialize<T>(resultsJson);
                    // 再次解析 results 字符串为 JSON 对象
                    return entity;
                }
               
            }
            catch(Exception ex)
            {
                throw ex;
            }
       
        }
        public T GetResultAsync<T>(string url, Dictionary<string, string> Headers, Dictionary<string, string> Pramas, bool ingoreNameCase = false) where T : class, new()
        {
            var en = new T();
            client.DefaultRequestHeaders.Clear();
            foreach (var kvp in Headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }
            var queryString = string.Join("&", Pramas.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            url = $"{url}?{queryString}";
            try
            {
                HttpResponseMessage response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                // 读取响应内容
                responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement data = root.GetProperty("data");

                    // 提取并解析 results 字段的内容
                    string resultsJson = data.ToString();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = ingoreNameCase //
                    };
                    var entity = JsonSerializer.Deserialize<T>(resultsJson );
                    // 再次解析 results 字符串为 JSON 对象
                    return entity;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string GetResultStringAsync(string url, Dictionary<string, string> Headers, Dictionary<string, string> Pramas, bool ingoreNameCase = false) 
        {
         
            foreach (var kvp in Headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }
            var queryString = string.Join("&", Pramas.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            url = $"{url}?{queryString}";
            try
            {
                HttpResponseMessage response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                // 读取响应内容
                responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement data = root.GetProperty("data");

                    // 提取并解析 results 字段的内容
                    string resultsJson = data.ToString();
                 
                    // 再次解析 results 字符串为 JSON 对象
                    return resultsJson;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public string GetResultMessageStringAsync(string url, Dictionary<string, string> Headers, Dictionary<string, string> Pramas, out bool sucess)
        {
            sucess = false;
            var responseBody = string.Empty;
            client.DefaultRequestHeaders.Clear();
            foreach (var kvp in Headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }
            var queryString = string.Join("&", Pramas.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            url = $"{url}?{queryString}";
            try
            {
                HttpResponseMessage response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
  
                 responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement data = root.GetProperty("data");

                    // 提取并解析 results 字段的内容
                    string resultsJson = data.ToString();
                    sucess=true;
                    // 再次解析 results 字符串为 JSON 对象
                    return resultsJson;
                }

            }
            catch (Exception ex)
            {
                sucess = false;
                var msg = responseBody.ToString();
                return msg;
            }

        }

        public List<T> GetResultListAsync<T>(string url, Dictionary<string, string> Headers, Dictionary<string, string>Pramas,bool ingoreNameCase=false) where T : class, new()
        {
            var en = new T();
            foreach (var kvp in Headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);

            }
            var queryString = string.Join("&", Pramas.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            url = $"{url}?{queryString}";
            try
            {
                HttpResponseMessage response = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                // 读取响应内容
                responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement data = root.GetProperty("data");

                    // 提取并解析 results 字段的内容
                    string resultsJson = data.ToString();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = ingoreNameCase //
                    };
                    var entity = JsonSerializer.Deserialize<List<T>>(resultsJson, options);
                    // 再次解析 results 字符串为 JSON 对象
                    return entity;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private KeyValueList<string, string> GetData(string responseBody)
        {
            KeyValueList<string, string> keyValues = new KeyValueList<string, string>();
            //解析result
            using (JsonDocument doc = JsonDocument.Parse(responseBody))
            {
                JsonElement root = doc.RootElement;
                JsonElement data = root.GetProperty("data");

                // 提取并解析 results 字段的内容
                string resultsJson = data.GetProperty("results").GetString();
                // 再次解析 results 字符串为 JSON 对象
                using (JsonDocument resultsDoc = JsonDocument.Parse(resultsJson))
                {
                    JsonElement resultsRoot = resultsDoc.RootElement;
                    try
                    {
                        foreach (JsonProperty property in resultsRoot.EnumerateObject())
                        {
                            string key = property.Name;
                            string errorMessages = property.Value.GetProperty("errorMessages").GetString();
                            var result = property.Value.GetProperty("success").GetBoolean();
                            if (result == false)
                            {
                                keyValues.Add(new KeyValue<string, string>(key, errorMessages));
                            }
                        }
                    }
                    catch
                    {
                        keyValues.Add(new KeyValue<string, string>("results", resultsJson));
                    }
                }
            }
            return keyValues;
        }
    }
}
