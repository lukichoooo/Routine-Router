using Domain.Common.ValueObjects;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.ValueObjects;

namespace Domain.Entities.Users
{
    public class UserState
    {
        public Guid Id { get; private set; }
        public Name Name { get; private set; }
        public PasswordHash PasswordHash { get; private set; }

        public int Version { get; private set; } = 0;


        public void Apply(UserCreated e)
        {
            Id = e.UserId;
            Name = e.Name;
            PasswordHash = e.PasswordHash;
            Version = 0;
        }

        public void Apply(UserUpdated e)
        {
            Name = e.Name;
            PasswordHash = e.PasswordHash;
            Version++;
        }

        public void Apply(UserVerified e)
        {
            Version++;
        }


#pragma warning disable CS8618
        internal UserState() { }
#pragma warning restore CS8618
    }
}
