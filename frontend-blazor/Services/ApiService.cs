using System.Net.Http.Json;
using Blazored.LocalStorage;
using LocalCRM.Blazor.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace LocalCRM.Blazor.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public ApiService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    private async Task AddAuthHeader()
    {
        var token = await _localStorage.GetItemAsync<string>("accessToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<T>> GetAsync<T>(string url)
    {
        await AddAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<T>>(url) ?? new();
    }

    public async Task<T?> GetByIdAsync<T>(string url)
    {
        await AddAuthHeader();
        return await _httpClient.GetFromJsonAsync<T>(url);
    }

    public async Task PostAsync<T>(string url, T data)
    {
        await AddAuthHeader();
        await _httpClient.PostAsJsonAsync(url, data);
    }

    public async Task PutAsync<T>(string url, T data)
    {
        await AddAuthHeader();
        await _httpClient.PutAsJsonAsync(url, data);
    }

    public async Task DeleteAsync(string url)
    {
        await AddAuthHeader();
        await _httpClient.DeleteAsync(url);
    }

    public async Task<bool> LoginAsync(LoginCommand command)
    {
        var result = await _httpClient.PostAsJsonAsync("api/auth/login", command);
        if (result.IsSuccessStatusCode)
        {
            var response = await result.Content.ReadFromJsonAsync<AuthResponse>();
            if (response != null && !response.PasswordChangeRequired)
            {
                await _localStorage.SetItemAsync("accessToken", response.AccessToken);
                await _localStorage.SetItemAsync("refreshToken", response.RefreshToken);
                ((Auth.CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.AccessToken);
                return true;
            }
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("accessToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        ((Auth.CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }
}
