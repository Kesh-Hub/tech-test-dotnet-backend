using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moonpig.PostOffice.Api.Controllers;
using Moonpig.PostOffice.Api.Services;
using Moonpig.PostOffice.Data;
using Moq;
using Shouldly;
using Xunit;

namespace Moonpig.PostOffice.Tests
{
    public class DespatchServiceTests
    {
        private readonly DespatchService _sut;

        public DespatchServiceTests()
        {
            var loggerMock = new Mock<ILogger<DespatchService>>();
            var repository = new Repository(new DbContext(), new Mock<ILogger<Repository>>().Object);

            // Use 'real' repository
            _sut = new DespatchService(loggerMock.Object, repository);
        }

        [Fact]
        public async Task OneProductWithLeadTimeOfOneDay()
        {
            //Act
            var date = await _sut.GetDespatchDateAsync(new List<int>() { 1 }, DateTime.Now);

            //Assert
            date.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(1));
        }

        [Fact]
        public async Task OneProductWithLeadTimeOfTwoDay()
        {
            // Act
            var date = await _sut.GetDespatchDateAsync(new List<int>() { 2 }, DateTime.Now);

            // Assert
            date.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(2));
        }

        [Fact]
        public async Task OneProductWithLeadTimeOfThreeDay()
        {
            // Act
            var date = await _sut.GetDespatchDateAsync(new List<int>() { 3 }, DateTime.Now);

            // Assert
            date.Date.Date.ShouldBe(DateTime.Now.Date.AddDays(3));
        }

        [Fact]
        public async Task SaturdayHasExtraTwoDays()
        {
            //Act
            var date = await _sut.GetDespatchDateAsync(new List<int>() { 1 }, new DateTime(2018, 1, 26));

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 26).Date.AddDays(3));
        }

        [Fact]
        public async Task SundayHasExtraDay()
        {
            //Act
            var date = await _sut.GetDespatchDateAsync(new List<int>() { 3 }, new DateTime(2018, 1, 25));

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 25).Date.AddDays(4));
        }
    }
}