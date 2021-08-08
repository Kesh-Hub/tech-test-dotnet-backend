using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moonpig.PostOffice.Api.Controllers;
using Moonpig.PostOffice.Api.Model;
using Moonpig.PostOffice.Api.Services;
using Moq;
using Shouldly;
using Xunit;

namespace Moonpig.PostOffice.Tests
{
    public class DespatchDateControllerTests
    {
        private readonly Mock<ILogger<DespatchDateController>> _loggerMock = new();
        private readonly Mock<IDespatchService> _despatchServiceMock = new();

        [Fact]
        public async Task ReturnsDespatchDate()
        {
            // Arrange
            var sut = new DespatchDateController(_loggerMock.Object, _despatchServiceMock.Object);
            var productIds = new List<int> {0};
            var orderDate = DateTime.Today;
            var despatchDate = new DespatchDate {Date = orderDate.AddDays(1)};

            _despatchServiceMock.Setup(x => x.GetDespatchDateAsync(productIds, orderDate)).ReturnsAsync(despatchDate);

            // Act
            var actionResult = await sut.Get(productIds, orderDate);

            // Assert
            actionResult.ShouldBeOfType(typeof(OkObjectResult));
            var result = (OkObjectResult) actionResult;
            Assert.Equal(despatchDate, result.Value);
        }

        [Fact]
        public async Task InvalidProductIds()
        {
            // Arrange
            var sut = new DespatchDateController(_loggerMock.Object, _despatchServiceMock.Object);

            // Act
            var actionResult = await sut.Get(null, DateTime.UtcNow);

            // Assert
            actionResult.ShouldBeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task InvalidOrderDate()
        {
            // Arrange
            var sut = new DespatchDateController(_loggerMock.Object, _despatchServiceMock.Object);

            // Act
            var actionResult = await sut.Get(new List<int> { 0 }, new DateTime());

            // Assert
            actionResult.ShouldBeOfType(typeof(BadRequestObjectResult));
        }
    }
}