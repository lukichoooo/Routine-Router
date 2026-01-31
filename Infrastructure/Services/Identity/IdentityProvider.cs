using Application.UseCases.Identity;

namespace Infrastructure.Services.Identity
{
    public class IdentityProvider : IIdentityProvider
    {
        public string GetCurrentUserName()
            => Environment.UserName;
    }
}
