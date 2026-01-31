using Domain.Entities.Users.ValueObjects;

namespace Application.UseCases.Identity
{
    public interface IIdentityProvider
    {
        public string GetCurrentUserName();
        public UserId GetCurrentUserId();
    }
}
