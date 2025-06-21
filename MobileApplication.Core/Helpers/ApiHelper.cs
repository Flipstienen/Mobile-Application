using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;
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

    public async Task<int> CreateOrderAsync<T>(object data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/order", content);

        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();

        int orderId = JsonConvert.DeserializeObject<Order>(responseJson).Id;
        content = new StringContent(orderId.ToString(), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync($"{_baseUrl}/api/DeliveryStates/StartDelivery?OrderId={orderId}", null);

        string resultJson = await result.Content.ReadAsStringAsync();
        return orderId;
    }

}