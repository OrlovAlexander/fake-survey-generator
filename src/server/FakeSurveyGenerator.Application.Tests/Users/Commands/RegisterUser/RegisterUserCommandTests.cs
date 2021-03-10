﻿using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeSurveyGenerator.Application.Common.Errors;
using FakeSurveyGenerator.Application.Common.Identity;
using FakeSurveyGenerator.Application.Users.Commands.RegisterUser;
using FakeSurveyGenerator.Data;
using FluentAssertions;
using Moq;
using Xunit;

namespace FakeSurveyGenerator.Application.Tests.Users.Commands.RegisterUser
{
    public sealed class RegisterUserCommandTests : CommandTestBase
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IUserService> _mockUserService = new();

        public RegisterUserCommandTests()
        {
            _mockUserService.Setup(service => service.GetUserInfo(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestUser());
        }

        [Fact]
        public async Task GivenValidRegisterUserCommand_WhenCallingHandle_ThenResultShouldBeSuccessful()
        {
            var registerUserCommand = new RegisterUserCommand();

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserInfo(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestUser(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>()));

            var sut = new RegisterUserCommandHandler(mockUserService.Object, Context, Mapper);

            var result = await sut.Handle(registerUserCommand, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public async Task GivenValidRegisterUserCommand_WhenCallingHandle_ThenNewUserShouldBeReturned(string newUserId, string newUserDisplayName, string newUserEmailAddress)
        {
            var registerUserCommand = new RegisterUserCommand();

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserInfo(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TestUser(newUserId, newUserDisplayName, newUserEmailAddress));

            var sut = new RegisterUserCommandHandler(mockUserService.Object, Context, Mapper);

            var result = await sut.Handle(registerUserCommand, CancellationToken.None);

            result.Value.Id.Should().BePositive();
            result.Value.ExternalUserId.Should().Be(newUserId);
            result.Value.DisplayName.Should().Be(newUserDisplayName);
            result.Value.EmailAddress.Should().Be(newUserEmailAddress);
        }

        [Fact]
        public async Task GivenAlreadyExistingUser_WhenCallingHandle_ThenResultShouldBeFailure()
        {
            var registerUserCommand = new RegisterUserCommand();

            var sut = new RegisterUserCommandHandler(_mockUserService.Object, Context, Mapper);

            var result = await sut.Handle(registerUserCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task GivenAlreadyExistingUser_WhenCallingHandle_ThenUserAlreadyRegisteredErrorShouldBeReturned()
        {
            var registerUserCommand = new RegisterUserCommand();

            var sut = new RegisterUserCommandHandler(_mockUserService.Object, Context, Mapper);

            var result = await sut.Handle(registerUserCommand, CancellationToken.None);

            result.Error.Should().Be(Errors.General.UserAlreadyRegistered());
        }
    }
}
