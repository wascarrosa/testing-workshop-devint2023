using Xunit;
using LegacyApp;
using FluentAssertions;

namespace LegacyApp.Tests.Unit
{
    public class UserServiceTest
    {
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userService = new UserService();
        }


        [Fact]
        public void AddUser_ShouldNotCreateUser_WhenFirstNameIsEmpty()
        {
            // Arrange
            var user = new User
            {
                Firstname = "",
                Surname = "Smith",
                // Fill in other properties as necessary
            };

            // Act
            var result = _userService.AddUser(user.Firstname,user.Surname,user.EmailAddress,user.DateOfBirth,user.Id);

            // Assert
            result.Should().BeFalse();  // FluentAssertions 

        }

        [Fact]
        public void AddUser_ShouldNotCreateUser_WhenLastNameIsEmpty()
        {
            // Arrange
            var user = new User
            {
                Firstname = "John",
                Surname = "",
                // Fill in other properties as necessary
            };

            // Act
            var result = _userService.AddUser(user.Firstname, user.Surname, user.EmailAddress, user.DateOfBirth, user.Id);

            // Assert
            result.Should().BeFalse();  // FluentAssertions 
        }

        [Fact]



    }
}
