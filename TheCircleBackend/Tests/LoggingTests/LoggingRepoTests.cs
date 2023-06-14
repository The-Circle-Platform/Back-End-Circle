using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCircleBackend.DBInfra;
using TheCircleBackend.DBInfra.Repo;
using TheCircleBackend.Helper;

namespace Tests.LoggingTests
{
    public class LoggingRepoTests
    {
        [Fact]
        public void UserLogAddsLogItemWhenCalled()
        {
            //arrange
            var dbContextOptions = new DbContextOptionsBuilder<DomainContext>().UseInMemoryDatabase("DomainTestDB").Options;
            var domainContext = new DomainContext(dbContextOptions);
            var ilogger = new Mock<ILogger<EFLogItemRepo>>();
            var repo = new EFLogItemRepo(domainContext, ilogger.Object);
            var sut = new LogHelper(repo, ilogger.Object);
            //act
            sut.AddUserLog("127.0.0.1", "POST User", "1", "This is a test");
            //assert
            var result = domainContext.LogItem;
            Assert.True(result.Count() == 1);
            domainContext.Database.EnsureDeleted();
        }
    }
}
