namespace Application.UseCases.Identity
{
    public interface IIdentityProvider
    {
        public string GetCurrentUserName();
        public Guid GetCurrentUserId();
    }
}
