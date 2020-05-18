using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Web.Commands.Admin;

namespace Web.Handlers.Admin
{
    public class PasswordValidatorHandler : IRequestHandler<GetPasswordValidationQuery, IdentityResult>
    {
        private readonly UserManager<IdentityUser> UserManager;

        public PasswordValidatorHandler(UserManager<IdentityUser> userManager) => UserManager = userManager;

        public async Task<IdentityResult> Handle(GetPasswordValidationQuery request, CancellationToken cancellationToken)
        {
            var validator = new PasswordValidator<IdentityUser>();
            var result    = await validator.ValidateAsync(UserManager, request.User, request.Password);

            return result;
        }
    }
}