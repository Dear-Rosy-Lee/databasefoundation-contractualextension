using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace YuLinTu.Component.OriginalDataAccess.Core
{
    /// <summary>
    /// Provides a very small subset of the behaviour required by the
    /// "原始数据接入工具". The service focuses on configuration checking and
    /// stubs for querying contractors and uploading vector data. The heavy
    /// lifting (such as SM4 encryption, geometry conversion and task
    /// scheduling) can be adapted from <see
    /// cref="YuLinTu.Component.VectorDataDecoding"/>.
    /// </summary>
    internal class OriginalDataService
    {
        private readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Ensures that <see cref="Constants.AppIdValue"/> and
        /// <see cref="Constants.AppKeyValue"/> are available. When either value
        /// is missing an <see cref="InvalidOperationException"/> is thrown so
        /// the UI layer can prompt the user for input.
        /// </summary>
        public void EnsureAppInfo()
        {
            if (string.IsNullOrWhiteSpace(Constants.AppIdValue) ||
                string.IsNullOrWhiteSpace(Constants.AppKeyValue))
            {
                throw new InvalidOperationException("appid/appkey 未配置");
            }
        }

        /// <summary>
        /// Queries the provincial platform for contractors that still have
        /// parcels to upload. The server API is expected to return a JSON array
        /// which is projected to the <typeparamref name="T"/> type.
        /// </summary>
        public async Task<ObservableCollection<T>> QueryContractorsAsync<T>(string zoneCode)
        {
            EnsureAppInfo();
            var url = Constants.BaseUrl + "/contractors/pending";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(Constants.AppIdHeader, Constants.AppIdValue);
            request.Headers.Add(Constants.AppKeyHeader, Constants.AppKeyValue);
            request.Content = new StringContent(JsonSerializer.Serialize(new { dybm = zoneCode }));

            var response = await _client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var data = JsonSerializer.Deserialize<ObservableCollection<T>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data ?? new ObservableCollection<T>();
        }

        /// <summary>
        /// Uploads a collection of parcel geometries for the specified
        /// contractors. The <paramref name="incremental"/> flag indicates
        /// whether the upload should merge with existing data or overwrite it.
        /// The method only contains the HTTP plumbing – encryption and
        /// shapefile parsing should be supplied by the caller.
        /// </summary>
        public async Task UploadParcelsAsync<T>(IEnumerable<T> parcels, IEnumerable<string> contractorIds, bool incremental)
        {
            EnsureAppInfo();
            var url = Constants.BaseUrl + "/contractors/upload";
            var body = new
            {
                contractors = contractorIds,
                mode = incremental ? "incremental" : "overwrite",
                data = parcels
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(Constants.AppIdHeader, Constants.AppIdValue);
            request.Headers.Add(Constants.AppKeyHeader, Constants.AppKeyValue);
            request.Content = new StringContent(JsonSerializer.Serialize(body));
            var response = await _client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}

