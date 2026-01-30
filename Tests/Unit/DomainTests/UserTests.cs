using AutoFixture;
using Domain.Common;
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
            var user = new User([cmd]);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.DomainEvents, Is.Empty);
            Assert.That(user.Version, Is.EqualTo(cmd.Version));
        }

        [Test]
        public void Update_Success()
        {
            var cmd = _fix.Create<UserCreated>();
            var user = new User([cmd]);

            var newName = _fix.Create<Name>();
            var newPasswordHash = _fix.Create<PasswordHash>();
            user.Update(newName, newPasswordHash);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(user.DomainEvents, Has.Exactly(1).InstanceOf<UserUpdated>());
            Assert.That(user.Version, Is.EqualTo(cmd.Version + 1));
        }


        [Test]
        public void Verify_Success()
        {
            var cmd = _fix.Create<UserCreated>();
            var user = new User([cmd]);

            user.Verify(cmd.Name, cmd.PasswordHash);

            Assert.That(user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(user.DomainEvents, Has.Exactly(1).InstanceOf<UserVerified>());
            Assert.That(user.Version, Is.EqualTo(cmd.Version + 1));
        }


        [Test]
        public void Verify_Fail_WrongCredentials()
        {
            var cmd = _fix.Create<UserCreated>();
            var user = new User([cmd]);

            var wrongName = _fix.Create<Name>();
            var wrongPasswordHash = _fix.Create<PasswordHash>();

            Assert.Throws<WrongUserCredentialsException>(
                    () => user.Verify(wrongName, wrongPasswordHash));

            Assert.That(user.DomainEvents, Is.Empty);
            Assert.That(user.Version, Is.EqualTo(cmd.Version));
        }

        // ----------- STATE TESTS

        [Test]
        public void State_Create_Success()
        {
            var cmd = _fix.Create<UserCreated>();
            var state = new UserState();
            state.Apply(cmd);

            Assert.That(state, Is.Not.Null);
            Assert.That(state.Name, Is.EqualTo(cmd.Name));
            Assert.That(state.PasswordHash, Is.EqualTo(cmd.PasswordHash));
        }

        [Test]
        public void State_Update_Success()
        {
            var cmd = _fix.Create<UserCreated>();
            var state = new UserState();
            state.Apply(cmd);

            var newName = _fix.Create<Name>();
            var newPasswordHash = _fix.Create<PasswordHash>();
            var updateEvent = new UserUpdated(
                    state.Id,
                    100,
                    Clock.Now,
                    newName,
                    newPasswordHash);
            state.Apply(updateEvent);

            Assert.That(state, Is.Not.Null);
            Assert.That(state.Name, Is.EqualTo(newName));
            Assert.That(state.PasswordHash, Is.EqualTo(newPasswordHash));
        }



        [Test]
        public void State_Verify()
        {
            var cmd = _fix.Create<UserCreated>();
            var state = new UserState();
            state.Apply(cmd);
            var verifyEvent = _fix.Create<UserVerified>();
            state.Apply(verifyEvent);
        }

    }
}
