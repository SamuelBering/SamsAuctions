using System;
using System.Threading.Tasks;
using SamsAuctions.Models;
using Xunit;
using Moq;
using SamsAuctions.DAL;
using System.Security.Claims;
using SamsAuctions.BL;
using Microsoft.AspNetCore.Identity;

namespace SamsAuctionsTests
{

    public class AuctionsTests
    {

        [Theory]
        [InlineData("User must have created this auction", true, 1, "UserName2")]
        [InlineData("User must be in role: Admin", false, 0, "")]
        public async Task AddOrUpdateAuction_ReturnsInvalidOperationException(string errorMessage, bool isAdmin, int auctionId, string createdBy)
        {

            // Arrange
            var expectedErrorMessage = errorMessage;

            var mockRepo = new Mock<IAuctionsRepository>();
            mockRepo.Setup(repo => repo.AddOrUpdateAuction(It.IsAny<Auction>())).Returns(Task.CompletedTask).Verifiable();

            var userStoreMock = new Mock<IUserStore<AppUser>>();


            var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(userManager => userManager.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult(GetTestAppUser()));

            var auctions = new Auctions(mockRepo.Object, userManagerMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(user => user.IsInRole("Admin")).Returns(isAdmin);

            var auction = new Auction()
            {
                AuktionID = auctionId,
                SkapadAv = createdBy,
            };

            //Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {


                await auctions.AddOrUpdateAuction(auction, userMock.Object);

            });

            // Assert
            Assert.Equal(expectedErrorMessage, exception.Message);

        }

        private AppUser GetTestAppUser()
        {
            return new AppUser
            {
                FirstName = "FirstName",
                LastName = "LastName",
                UserName = "UserName"

            };
        }

    }


}
