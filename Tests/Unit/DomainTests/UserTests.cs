using AutoFixture;
using Domain.Common.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.Exceptions;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace DomainTests
{
    [TestFixture]
    public class UserTests
    {
        private readonly Fixture _fix = new();

        [OneTimeSetUp]
        public void Setup()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public void Create_Success()
        {
            var cmd = _fix.Create<UserCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var user = new User(commands);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(user.DomainEvents, Is.EquivalentTo(commands));
        }

        [Test]
        public void Update_Success()
        {
            var cmd = _fix.Create<UserCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var user = new User(commands);

            var newName = _fix.Create<Name>();
            var newPasswordHash = _fix.Create<PasswordHash>();
            user.Update(newName, newPasswordHash);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(user.DomainEvents, Has.Exactly(1).InstanceOf<UserCreated>());
            Assert.That(user.DomainEvents, Has.Exactly(1).InstanceOf<UserUpdated>());
        }


        [Test]
        public void Verify_Success()
        {
            var cmd = _fix.Create<UserCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var user = new User(commands);

            user.Verify(cmd.Name, cmd.PasswordHash);

            Assert.That(user.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(user.DomainEvents, Has.Exactly(1).InstanceOf<UserCreated>());
            Assert.That(user.DomainEvents, Has.Exactly(1).InstanceOf<UserVerified>());
        }


        [Test]
        public void Verify_Fail_WrongCredentials()
        {
            var cmd = _fix.Create<UserCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var user = new User(commands);

            var wrongName = _fix.Create<Name>();
            var wrongPasswordHash = _fix.Create<PasswordHash>();

            Assert.Throws<WrongUserCredentialsException>(
                    () => user.Verify(wrongName, wrongPasswordHash));

            Assert.That(user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(user.DomainEvents, Is.EquivalentTo(commands));
        }
    }
}
