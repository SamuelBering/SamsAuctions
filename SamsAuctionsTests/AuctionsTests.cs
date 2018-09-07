﻿using System;
using System.Threading.Tasks;
using SamsAuctions.Models;
using Xunit;
using Moq;
using SamsAuctions.DAL;
using System.Security.Claims;
using SamsAuctions.BL;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
namespace SamsAuctionsTests
{
    public class AuctionsTests
    {
        private class MocksForUpdateAuctionTests
        {
            public Auction Auction { get; set; }
            public Mock<IAuctionsRepository> MockRepo { get; set; }
            public Mock<ClaimsPrincipal> MockUser { get; set; }
            public Mock<UserManager<AppUser>> MockUserManager { get; set; }
        }
        private MocksForUpdateAuctionTests CreateMocksForAddOrUpdateAuctionTests(int auctionId, string firstName, string lastName, string userName, string createdBy, bool isAdmin)
        {
            var auction = new Auction()
            {
                AuktionID = auctionId,
                SkapadAv = createdBy,
            };
            var mockRepo = new Mock<IAuctionsRepository>();
            mockRepo.Setup(repo => repo.AddOrUpdateAuction(auction)).Returns(Task.CompletedTask).Verifiable();
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(user => user.IsInRole("Admin")).Returns(isAdmin);
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(userManager => userManager.GetUserAsync(userMock.Object)).Returns(Task.FromResult(GetTestAppUser(firstName, lastName, userName)));
            return new MocksForUpdateAuctionTests
            {
                Auction = auction,
                MockRepo = mockRepo,
                MockUser = userMock,
                MockUserManager = userManagerMock
            };
        }
        [Theory]
        [InlineData(0, "TestUser")]
        [InlineData(1, "TestUser")]
        public async Task AddOrUpdateAuction_CallsRepositoryAddOrUpdateAuction_WhenUserHasPermissions(int auctionId, string createdBy)
        {
            //Arrange
            var mocks = CreateMocksForAddOrUpdateAuctionTests(auctionId, "", "", createdBy, createdBy, true);
            var auctions = new Auctions(mocks.MockRepo.Object, mocks.MockUserManager.Object);
            //Act
            await auctions.AddOrUpdateAuction(mocks.Auction, mocks.MockUser.Object);
            //Assert
            mocks.MockRepo.Verify(r => r.AddOrUpdateAuction(mocks.Auction));
            Assert.Equal("", "");
        }
        //[Theory]
        //[InlineData(0, "TestUser")]
        //[InlineData(1, "TestUser")]
        //public async Task AddOrUpdateAuction_CallsRepositoryAddOrUpdateAuction_WhenUserHasPermissions(int auctionId, string createdBy)
        //{
        //    // Arrange
        //    //var expectedErrorMessage = errorMessage;
        //    var auction = new Auction()
        //    {
        //        AuktionID = auctionId,
        //        SkapadAv = createdBy,
        //    };
        //    var mockRepo = new Mock<IAuctionsRepository>();
        //    mockRepo.Setup(repo => repo.AddOrUpdateAuction(auction)).Returns(Task.CompletedTask).Verifiable();
        //    var userMock = new Mock<ClaimsPrincipal>();
        //    userMock.Setup(user => user.IsInRole("Admin")).Returns(true);
        //    var userStoreMock = new Mock<IUserStore<AppUser>>();
        //    var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        //    userManagerMock.Setup(userManager => userManager.GetUserAsync(userMock.Object)).Returns(Task.FromResult(GetTestAppUser("", "", createdBy)));
        //    var auctions = new Auctions(mockRepo.Object, userManagerMock.Object);
        //    //Act
        //    await auctions.AddOrUpdateAuction(auction, userMock.Object);
        //    mockRepo.Verify(r => r.AddOrUpdateAuction(auction));
        //    Assert.Equal("", "");
        //}
        [Theory]
        [InlineData("User must have created this auction", true, 1, "UserName2")]
        [InlineData("User must be in role: Admin", false, 0, "")]
        public async Task AddOrUpdateAuction_ThrowsInvalidOperationException_WhenUserHasNotPermissions(string errorMessage, bool isAdmin, int auctionId, string createdBy)
        {
            // Arrange
            var mocks = CreateMocksForAddOrUpdateAuctionTests(auctionId, "FirstName", "LastName", "UserName", createdBy, isAdmin);
            var expectedErrorMessage = errorMessage;
            var auctions = new Auctions(mocks.MockRepo.Object, mocks.MockUserManager.Object);
            //Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await auctions.AddOrUpdateAuction(mocks.Auction, mocks.MockUser.Object);
            });
            // Assert
            Assert.Equal(expectedErrorMessage, exception.Message);
        }
        //[Theory]
        //[InlineData("User must have created this auction", true, 1, "UserName2")]
        //[InlineData("User must be in role: Admin", false, 0, "")]
        //public async Task AddOrUpdateAuction_ThrowsInvalidOperationException_WhenUserHasNotPermissions(string errorMessage, bool isAdmin, int auctionId, string createdBy)
        //{
        //    // Arrange
        //    var expectedErrorMessage = errorMessage;
        //    var mockRepo = new Mock<IAuctionsRepository>();
        //    mockRepo.Setup(repo => repo.AddOrUpdateAuction(It.IsAny<Auction>())).Returns(Task.CompletedTask).Verifiable();
        //    var userMock = new Mock<ClaimsPrincipal>();
        //    userMock.Setup(user => user.IsInRole("Admin")).Returns(isAdmin);
        //    var userStoreMock = new Mock<IUserStore<AppUser>>();
        //    var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        //    userManagerMock.Setup(userManager => userManager.GetUserAsync(userMock.Object)).Returns(Task.FromResult(GetTestAppUser("FirstName", "LastName", "UserName")));
        //    var auctions = new Auctions(mockRepo.Object, userManagerMock.Object);
        //    var auction = new Auction()
        //    {
        //        AuktionID = auctionId,
        //        SkapadAv = createdBy,
        //    };
        //    //Act
        //    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        //    {
        //        await auctions.AddOrUpdateAuction(auction, userMock.Object);
        //    });
        //    // Assert
        //    Assert.Equal(expectedErrorMessage, exception.Message);
        //}
        private IList<Auction> GetAuctions(string createdBy, int groupCode = 1)
        {
            return new List<Auction>
            {
                new Auction
                {
                    AuktionID=1,
                    Titel="Auktion1",
                    StartDatum=new DateTime(2018,05,01),
                    SlutDatum=new DateTime(2018,05,11),
                    SkapadAv=createdBy,
                    Gruppkod=groupCode,
                },
                 new Auction
                {
                    AuktionID=2,
                    Titel="Auktion2",
                    StartDatum=new DateTime(2018,06,01),
                    SlutDatum=new DateTime(2018,06,11),
                    SkapadAv="AnnanAnvändare",
                    Gruppkod=groupCode,
                },
                  new Auction
                {
                    AuktionID=3,
                    Titel="Auktion3",
                    StartDatum=new DateTime(2018,07,01),
                    SlutDatum=new DateTime(2018,07,11),
                    SkapadAv=createdBy,
                    Gruppkod=groupCode,
                },
                    new Auction
                {
                    AuktionID=4,
                    Titel="Auktion4",
                    StartDatum=new DateTime(5000,07,01),
                    SlutDatum=new DateTime(5000,07,11),
                    SkapadAv=createdBy,
                    Gruppkod=groupCode,
                }
            };
        }
        private IList<Bid> GetBids(int auctionId)
        {
            return (new List<Bid>
            {
                new Bid
                {
                    BudID=1,
                    AuktionID=1,
                    Summa=100,
                    Budgivare="Budgivare1",
                },
                 new Bid
                {
                    BudID=2,
                    AuktionID=2,
                    Summa=200,
                    Budgivare="Budgivare2",
                },
                  new Bid
                {
                    BudID=3,
                    AuktionID=2,
                    Summa=300,
                    Budgivare="Budgivare3",
                },
            }).Where(b => b.AuktionID == auctionId).ToList();
        }
        private MocksForGetClosedAuctionsTests CreateMocksForGetClosedAuctionsTests(int groupCode, string firstName, string lastName, string userName, string createdBy)
        {
            var mockRepo = new Mock<IAuctionsRepository>();
            mockRepo.Setup(repo => repo.GetAllAuctions(groupCode)).Returns(Task.FromResult(GetAuctions(userName))).Verifiable();
            var userMock = new Mock<ClaimsPrincipal>();
            //userMock.Setup(user => user.IsInRole("Admin")).Returns(isAdmin);
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(userManager => userManager.GetUserAsync(userMock.Object)).Returns(Task.FromResult(GetTestAppUser(firstName, lastName, userName)));
            return new MocksForGetClosedAuctionsTests
            {
                MockRepo = mockRepo,
                MockUser = userMock,
                MockUserManager = userManagerMock
            };
        }
        private class MocksForGetClosedAuctionsTests : MocksForTests
        {
        }
        private class MocksForTests
        {
            public Mock<IAuctionsRepository> MockRepo { get; set; }
            public Mock<ClaimsPrincipal> MockUser { get; set; }
            public Mock<UserManager<AppUser>> MockUserManager { get; set; }
        }
        private MocksForTests CreateMocksForGetAllAuctions(int groupCode, string firstName, string lastName, string userName, string createdBy)
        {
            var mockRepo = new Mock<IAuctionsRepository>();
            mockRepo.Setup(repo => repo.GetAllAuctions(groupCode)).Returns(Task.FromResult(GetAuctions(userName))).Verifiable();
            mockRepo.Setup(repo => repo.GetAllBids(groupCode, 1)).Returns(Task.FromResult(GetBids(1))).Verifiable();
            mockRepo.Setup(repo => repo.GetAllBids(groupCode, 2)).Returns(Task.FromResult(GetBids(2))).Verifiable();
            mockRepo.Setup(repo => repo.GetAllBids(groupCode, 3)).Returns(Task.FromResult(GetBids(3))).Verifiable();
            mockRepo.Setup(repo => repo.GetAllBids(groupCode, 4)).Returns(Task.FromResult(GetBids(4))).Verifiable();
            var userMock = new Mock<ClaimsPrincipal>();
            //userMock.Setup(user => user.IsInRole("Admin")).Returns(isAdmin);
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(userManager => userManager.GetUserAsync(userMock.Object)).Returns(Task.FromResult(GetTestAppUser(firstName, lastName, userName)));
            return new MocksForTests
            {
                MockRepo = mockRepo,
                MockUser = userMock,
                MockUserManager = userManagerMock
            };
        }
        [Theory]
        [InlineData(2018, 05, 11, 2018, 07, 11, 2, true)]
        [InlineData(2018, 05, 12, 2018, 07, 11, 2, false)]
        [InlineData(2018, 05, 12, 2018, 07, 10, 1, false)]
        public async Task GetClosedAuctions_ReturnsListOfAuctionsWithinTimePeriod(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay, int expectedCount, bool isOwnAction)
        {
            var groupCode = 1;
            //arrange
            var mocks = CreateMocksForGetClosedAuctionsTests(groupCode, "FirstName", "LastName", "UserName", "AnnanAnvändare");
            var auctions = new Auctions(mocks.MockRepo.Object, mocks.MockUserManager.Object);
            //act
            var result = await auctions.GetClosedAuctions(groupCode, new DateTime(startYear, startMonth, startDay),
                new DateTime(endYear, endMonth, endDay), mocks.MockUser.Object, isOwnAction);
            //assert
            Assert.Equal(expectedCount, result.Count);
        }
        [Fact]
        public async Task GetAllAuctions_ReturnsListOfAllAuctionsWithCorrectData()
        {
            var groupCode = 1;
            //arrange
            var mocks = CreateMocksForGetAllAuctions(groupCode, "FirstName", "LastName", "UserName", "AnnanAnvändare");
            var auctions = new Auctions(mocks.MockRepo.Object, mocks.MockUserManager.Object);
            //act
            var result = await auctions.GetAllAuctions(groupCode, mocks.MockUser.Object);
            //assert
            Assert.Equal(4, result.Count);
            Assert.True(result[0].AnvandarenFarUppdatera);
            Assert.False(result[0].AnvandarenFarTaBort);
            Assert.False(result[1].AnvandarenFarUppdatera);
            Assert.True(result[2].AnvandarenFarTaBort);
            Assert.False(result[2].ArOppen);
            Assert.True(result[3].ArOppen);
        }
        [Fact]
        public async Task GetAllBids_CallsRepositoryGetAllBidsWithCorrectArguments()
        {
            //arrange
            var groupCode = 1;
            var auctionId = 1;
            var mockRepo = new Mock<IAuctionsRepository>();
            mockRepo.Setup(repo => repo.GetAllBids(groupCode, auctionId)).Returns(Task.FromResult((IList<Bid>)null)).Verifiable();
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var auctions = new Auctions(mockRepo.Object, userManagerMock.Object);
            //act
            await auctions.GetAllBids(auctionId, groupCode);
            //assert
            mockRepo.Verify(repo => repo.GetAllBids(groupCode, auctionId));
        }

        [Fact]
        public async Task GetAuction_CallsRepositoryGetAuctionWithCorrectArguments()
        {
            //arrange
            var groupCode = 1;
            var auctionId = 1;
            var mockRepo = new Mock<IAuctionsRepository>();
            mockRepo.Setup(repo => repo.GetAuction(auctionId, groupCode)).Returns(Task.FromResult((Auction)null)).Verifiable();
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var auctions = new Auctions(mockRepo.Object, userManagerMock.Object);
            //act
            await auctions.GetAuction(auctionId, groupCode);
            //assert
            mockRepo.Verify(repo => repo.GetAuction(auctionId, groupCode));
        }


        //[Fact]
        //public async Task AddBid_ThrowsInvalidOperationException_WhenBidAmountIsLowerThanOrEqualWithHighestBid()
        //{
        //    //arrange
        //    var groupCode = 1;

        //    var bid = new Bid
        //    {
        //        AuktionID = 1,
        //        Summa = 1,
        //        Budgivare = "TestBudgivare",
        //        BudID = 97,
        //    };
        //    var auction = GetAuctions("testUserName", groupCode).First();


        //    var mockRepo = new Mock<IAuctionsRepository>();
        //    mockRepo.Setup(repo => repo.GetAuction(bid.AuktionID, groupCode)).Returns(Task.FromResult(auction)).Verifiable();
        //    mockRepo.Setup(repo => repo.GetAllBids(groupCode, auction.AuktionID)).Returns(Task.FromResult(GetBids(auction.AuktionID))).Verifiable();
        //    //GetAllBids
        //    var userStoreMock = new Mock<IUserStore<AppUser>>();
        //    var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        //    var auctions = new Auctions(mockRepo.Object, userManagerMock.Object);
        //    //act
        //    await auctions.GetAuction(auctionId, groupCode);
        //    //assert
        //    mockRepo.Verify(repo => repo.GetAuction(auctionId, groupCode));
        //}


        private AppUser GetTestAppUser(string firstName, string lastName, string userName)
        {
            return new AppUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
            };
        }
    }
}
