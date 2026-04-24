using MediatR;
using Velora.Application.Common.Interfaces;

namespace Velora.Application.Auth.Commands;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<(bool Succeeded, string UserId)>;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, (bool Succeeded, string UserId)>
{
    private readonly IIdentityService _identityService;

    public RegisterUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<(bool Succeeded, string UserId)> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.CreateUserAsync(request.Email, request.Password, request.FirstName, request.LastName);
    }
}
