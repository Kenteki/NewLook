using Blazored.LocalStorage;
using NewLook.Models.DTOs.Auth;

namespace NewLook.Services
{
    public class AuthStateService
    {
        private readonly ILocalStorageService _localStorage;
        public event Action? OnChange;

        public AuthStateService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("token");
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            return await _localStorage.GetItemAsync<UserDto>("user");
        }

        public async Task LoginAsync(AuthResponseDto response)
        {
            await _localStorage.SetItemAsync("token", response.Token);
            await _localStorage.SetItemAsync("user", response.User);
            NotifyStateChanged();
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("token");
            await _localStorage.RemoveItemAsync("user");
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}