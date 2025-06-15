using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobileApplication.Core.Helpers;

public sealed class ApiHelper
{
    private static readonly Lazy<ApiHelper> _instance = new Lazy<ApiHelper>(() => new ApiHelper());
    public static ApiHelper Instance => _instance.Value;

    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _apiKey;

    private ApiHelper()
    {
        _httpClient = new HttpClient();
        _baseUrl = EnvHelper.Instance.GetEnvironmentVariable("API_BASE_URL", "http://localhost:5000");
        _apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            Console.WriteLine("Warning: API_KEY is not set.");
        }

        _httpClient.DefaultRequestHeaders.Add("ApiKey", _apiKey);
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json);
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content);
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(responseJson);
    }
}