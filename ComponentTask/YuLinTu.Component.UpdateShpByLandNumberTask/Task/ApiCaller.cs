using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace YuLinTu.Component.UpdateShpByLandNumberTask
{
    public class ApiCaller
    {
        private const int PollingInterval =5000; // 轮询间隔时间（毫秒）
        private const int Timeout = 1800000; // 超时时间（毫秒） 

        public HttpClient client { get; set; }

        public string ErrorInfo {  get; set; }

        public string PostGetTaskIDAsync(string token,string url, string jsonData)
        {
            try
            {
                
                // 将JSON数据转换为HttpContent
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                content.Headers.Add("token", token);
                // 发送 POST 请求
                HttpResponseMessage response =  client.PostAsync(url, content).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
              
                // 读取响应内容
                var responseBody =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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

        public  KeyValueList<string, string> PostGetResultAsync(string token, string url, string taskID)
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
                    HttpResponseMessage response =  client.GetAsync(url1).ConfigureAwait(false).GetAwaiter().GetResult(); 
                    response.EnsureSuccessStatusCode();
                    // 读取响应内容
                    responseBody =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); 
                    if (response.IsSuccessStatusCode)
                    {
                        using (JsonDocument doc = JsonDocument.Parse(responseBody))
                        {
                            JsonElement root = doc.RootElement;
                            JsonElement data = root.GetProperty("data");
                            if (data.ValueKind!= JsonValueKind.Null)
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
        private KeyValueList<string,string> GetData(string responseBody)
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
            }
            return keyValues;
        }
    }
}
