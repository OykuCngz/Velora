namespace Velora.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName);
    Task<(bool Succeeded, string Token)> LoginAsync(string userName, string password);
    Task<bool> CheckPasswordAsync(string userName, string password);
}
