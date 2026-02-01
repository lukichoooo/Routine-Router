using AutoFixture;
using Domain.Common;
using Domain.Common.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.Exceptions;
using Domain.Entities.Users.ValueObjects;
using FixtureProvider;

namespace DomainTests
{
    [TestFixture]
    public class UserTests
    {
        private readonly Fixture _fix = FixtureFactory.GetFixture();

        [Test]
        public void Create_Success()
        {
            var evt = _fix.Create<UserCreated>();
            var user = new User([evt]);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.DomainEvents, Is.Empty);
            Assert.That(user.Version, Is.EqualTo(evt.Version));
        }

        [Test]
        public void Update_Success()
        {
            var evt = _fix.Create<UserCreated>();
            var user = new User([evt]);

            var newName = _fix.Create<Name>();
            var newPasswordHash = _fix.Create<PasswordHash>();
            user.Update(newName, newPasswordHash);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(user.DomainEvents, Has.Exactly(1).InstanceOf<UserUpdated>());
            Assert.That(user.Version, Is.EqualTo(evt.Version + 1));
        }


        [Test]
        public void Verify_Success()
        {
            var evt = _fix.Create<UserCreated>();
            var user = new User([evt]);

            user.Verify(evt.Name, evt.PasswordHash);

            Assert.That(user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(user.DomainEvents, Has.Exactly(1).InstanceOf<UserVerified>());
            Assert.That(user.Version, Is.EqualTo(evt.Version + 1));
        }


        [Test]
        public void Verify_Fail_WrongCredentials()
        {
            var evt = _fix.Create<UserCreated>();
            var user = new User([evt]);

            var wrongName = _fix.Create<Name>();
            var wrongPasswordHash = _fix.Create<PasswordHash>();

            Assert.Throws<WrongUserCredentialsException>(
                    () => user.Verify(wrongName, wrongPasswordHash));

            Assert.That(user.DomainEvents, Is.Empty);
            Assert.That(user.Version, Is.EqualTo(evt.Version));
        }

        // ----------- STATE TESTS

        [Test]
        public void State_Create_Success()
        {
            var evt = _fix.Create<UserCreated>();
            var state = new UserState();
            state.Apply(evt);

            Assert.That(state, Is.Not.Null);
            Assert.That(state.Name, Is.EqualTo(evt.Name));
            Assert.That(state.PasswordHash, Is.EqualTo(evt.PasswordHash));
        }

        [Test]
        public void State_Update_Success()
        {
            var evt = _fix.Create<UserCreated>();
            var state = new UserState();
            state.Apply(evt);

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
            var evt = _fix.Create<UserCreated>();
            var state = new UserState();
            state.Apply(evt);
            var verifyEvent = _fix.Create<UserVerified>();
            state.Apply(verifyEvent);
        }

    }
}
