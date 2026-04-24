using MediatR;
using Velora.Application.Common.Interfaces;

namespace Velora.Application.Auth.Commands;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<(bool Succeeded, string Token)>;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, (bool Succeeded, string Token)>
{
    private readonly IIdentityService _identityService;

    public LoginUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<(bool Succeeded, string Token)> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.Email, request.Password);
    }
}
