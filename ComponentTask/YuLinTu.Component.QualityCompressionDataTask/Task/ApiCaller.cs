using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace YuLinTu.Component.QualityCompressionDataTask
{
    public class ApiCaller
    {

        public HttpClient client { get; set; }

        public async System.Threading.Tasks.Task<string> PostGetTaskIDAsync(string token,string url, string jsonData)
        {
            try
            {
                
                // 将JSON数据转换为HttpContent
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                content.Headers.Add("token", token);
                // 发送 POST 请求
                HttpResponseMessage response = await client.PostAsync(url, content);

                response.EnsureSuccessStatusCode();
              
                // 读取响应内容
                var responseBody = await response.Content.ReadAsStringAsync();
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
        public async System.Threading.Tasks.Task<KeyValueList<string,string>> PostGetResultAsync(string token,string url, string taskID)
        {
            try
            {
                url = $"{url}?jobId={taskID}";

                client.DefaultRequestHeaders.Add("accept", "*/*");
                client.DefaultRequestHeaders.Add("token", token);
                // 发送 Get 请求
                HttpResponseMessage response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                // 读取响应内容
                var responseBody = await response.Content.ReadAsStringAsync();

                KeyValueList<string,string> keyValues = new KeyValueList<string,string>();
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
                            if (result==false)
                            {
                                keyValues.Add(new KeyValue<string, string>(key, errorMessages));
                            }
                        }
                    }
                }
                return keyValues;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
                return null;
            }
        }
    }
}
