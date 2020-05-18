using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Web.Commands.Admin
{
    public class GetPasswordValidationQuery : IRequest<IdentityResult>
    {
        public IdentityUser? User     { get; set; }
        public string           Password { get; set; }

        public GetPasswordValidationQuery(IdentityUser? user, string password)
        {
            User     = user;
            Password = password;
        }
    }
}