using Application.UseCases.Identity;

namespace Infrastructure.Services
{
    public class IdentityProvider : IIdentityProvider
    {
        public string GetCurrentUserName()
            => Environment.UserName;
    }
}
