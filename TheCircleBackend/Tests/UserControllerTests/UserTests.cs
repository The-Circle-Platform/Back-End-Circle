using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TheCircleBackend.Controllers;
using TheCircleBackend.DBInfra;
using TheCircleBackend.DBInfra.Repo;
using TheCircleBackend.Domain.Models;

namespace Tests.UserControllerTests
{
   
    public class UserTests
    {
        [Fact]
        public void GetAllWebsiteUsersReturns2UsersIfTheDatabaseContainsTwoUsers()
        {
            //arrange
            var dbContextOptions = new DbContextOptionsBuilder<DomainContext>().UseInMemoryDatabase("DomainTestDB").Options;
            var domainContext = new DomainContext(dbContextOptions);
            var ilogger = new Mock<ILogger<EFWebsiteUserRepo>>();
            var sut = new EFWebsiteUserRepo(domainContext, ilogger.Object);
            var testUser = new WebsiteUser()
            {
                Id = 1,
                IsOnline = true,
                UserName = "henk"
            };
            var testUser2 = new WebsiteUser()
            {
                Id = 2,
                IsOnline = false,
                UserName = "henk2"
            };
            domainContext.Add(testUser);
            domainContext.Add(testUser2);
            domainContext.SaveChanges();

            //act
            var result = sut.GetAllWebsiteUsers();
            //assert
            Assert.True(result.Count() == 2);

            domainContext.Database.EnsureDeleted();
        }

        [Fact]
        public void GetByIdIdReturnsUser1IfIdIs1AndUserWithIdOneExists()
        {
            //arrange
            var dbContextOptions = new DbContextOptionsBuilder<DomainContext>().UseInMemoryDatabase("DomainTestDB").Options;
            var domainContext = new DomainContext(dbContextOptions);
            var ilogger = new Mock<ILogger<EFWebsiteUserRepo>>();
            var sut = new EFWebsiteUserRepo(domainContext, ilogger.Object);
            var testUser = new WebsiteUser()
            {
                Id = 1,
                IsOnline = true,
                UserName = "henk"
            };
            var testUser2 = new WebsiteUser()
            {
                Id = 2,
                IsOnline = false,
                UserName = "henk2"
            };
            domainContext.Add(testUser);
            domainContext.Add(testUser2);
            domainContext.SaveChanges();
            //act
            var result = sut.GetById(1);
            //assert
            Assert.True(result == testUser);
            
            domainContext.Database.EnsureDeleted();

        }

        [Fact]
        public void GetByIdReturnsNullIfUserDoesNotExist()
        {
            //arrange
            var dbContextOptions = new DbContextOptionsBuilder<DomainContext>().UseInMemoryDatabase("DomainTestDB").Options;
            var domainContext = new DomainContext(dbContextOptions);
            var ilogger = new Mock<ILogger<EFWebsiteUserRepo>>();
            var sut = new EFWebsiteUserRepo(domainContext, ilogger.Object);
            var testUser = new WebsiteUser()
            {
                Id = 1,
                IsOnline = true,
                UserName = "henk"
            };
            var testUser2 = new WebsiteUser()
            {
                Id = 2,
                IsOnline = false,
                UserName = "henk2"
            };
            domainContext.Add(testUser);
            domainContext.Add(testUser2);
            domainContext.SaveChanges();
            //act
            var result = sut.GetById(3);
            //assert
            Assert.Null(result);

            domainContext.Database.EnsureDeleted();
        }

        [Fact]
        public void AddAddsUser()
        {
            //arrange
            var dbContextOptions = new DbContextOptionsBuilder<DomainContext>().UseInMemoryDatabase("DomainTestDB").Options;
            var domainContext = new DomainContext(dbContextOptions);
            var ilogger = new Mock<ILogger<EFWebsiteUserRepo>>();
            var sut = new EFWebsiteUserRepo(domainContext, ilogger.Object);
            var testUser = new WebsiteUser()
            {
                Id = 1,
                IsOnline = true,
                UserName = "henk"
            };
            //act
            sut.Add(testUser);
            //assert
            Assert.Equal(1, domainContext.WebsiteUser.Count());
            domainContext.Database.EnsureDeleted();

        }

        [Fact]
        public void UpdateUpdatesUser()
        {
            var dbContextOptions = new DbContextOptionsBuilder<DomainContext>().UseInMemoryDatabase("DomainTestDB").Options;
            var domainContext = new DomainContext(dbContextOptions);
            var ilogger = new Mock<ILogger<EFWebsiteUserRepo>>();
            var sut = new EFWebsiteUserRepo(domainContext, ilogger.Object);
            var testUser = new WebsiteUser()
            {
                Id = 1,
                IsOnline = true,
                UserName = "henk"
            };
            domainContext.Add(testUser);
            domainContext.SaveChanges();

            var updatedUser = new WebsiteUser()
            {
                Id = 1,
                IsOnline = true,
                UserName = "ingrid"
            };
            //act
            sut.Update(updatedUser, 1);

            //assert
            var assertResult = domainContext.WebsiteUser.Where(u => u.Id == 1).First();
            Assert.Equal("ingrid", assertResult.UserName);
            domainContext.Database.EnsureDeleted();




        }
    }
}
