using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moonpig.PostOffice.Api.Services;
using Moonpig.PostOffice.Data;
using Moq;
using Shouldly;
using Xunit;

namespace Moonpig.PostOffice.Tests
{
    public class DespatchServiceTests
    {
        private readonly Mock<ILogger<DespatchService>> _loggerMock = new();
        private readonly Mock<IRepository> _repositoryMock = new();

        private static readonly Supplier SupplierWith1DayLeadTime = new() { SupplierId = 1, Name = "Test Supplier", LeadTime = 1 };
        private static readonly Supplier SupplierWith2DaysLeadTime = new() { SupplierId = 2, Name = "Test Supplier", LeadTime = 2 };
        private static readonly Supplier SupplierWith3DaysLeadTime = new() { SupplierId = 2, Name = "Test Supplier", LeadTime = 3 };
        private static readonly Supplier SupplierWith6DaysLeadTime = new() { SupplierId = 3, Name = "Test Supplier", LeadTime = 6 };
        private static readonly Supplier SupplierWith11DaysLeadTime = new() { SupplierId = 4, Name = "Test Supplier", LeadTime = 11 };


        #region Acceptance Criteria Tests
        [Fact]
        public async Task LeadTimeAddedToDespatchDate_1Day()
        {
            // Arrange
            var sut = new DespatchService(_loggerMock.Object, _repositoryMock.Object);
            var supplierList = new List<Supplier> { SupplierWith1DayLeadTime };
            _repositoryMock.Setup(x => x.GetSuppliersListForProductList(It.IsAny<List<int>>())).ReturnsAsync(supplierList);
            var orderDate = new DateTime(2018, 1, 1);

            //Act
            var date = await sut.GetDespatchDateAsync(new List<int>() { 1 }, orderDate);

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 2));
        }

        [Fact]
        public async Task LeadTimeAddedToDespatchDate_2Days()
        {
            // Arrange
            var sut = new DespatchService(_loggerMock.Object, _repositoryMock.Object);
            var supplierList = new List<Supplier> { SupplierWith2DaysLeadTime };
            _repositoryMock.Setup(x => x.GetSuppliersListForProductList(It.IsAny<List<int>>())).ReturnsAsync(supplierList);
            var orderDate = new DateTime(2018, 1, 1);

            //Act
            var date = await sut.GetDespatchDateAsync(new List<int>() { 1 }, orderDate);

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 3));
        }

        [Fact]
        public async Task LongestLeadTimeUsed()
        {
            // Arrange
            var sut = new DespatchService(_loggerMock.Object, _repositoryMock.Object);
            var supplierList = new List<Supplier> { SupplierWith1DayLeadTime, SupplierWith2DaysLeadTime };
            _repositoryMock.Setup(x => x.GetSuppliersListForProductList(It.IsAny<List<int>>())).ReturnsAsync(supplierList);
            var orderDate = new DateTime(2018, 1, 1);

            //Act
            var date = await sut.GetDespatchDateAsync(new List<int>() { 1 }, orderDate);

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 3));
        }

        [Fact]
        public async Task LeadTimeNotCountedOverWeekend_FridayOrder()
        {
            // Arrange
            var sut = new DespatchService(_loggerMock.Object, _repositoryMock.Object);
            var supplierList = new List<Supplier> { SupplierWith1DayLeadTime };
            _repositoryMock.Setup(x => x.GetSuppliersListForProductList(It.IsAny<List<int>>())).ReturnsAsync(supplierList);
            var orderDate = new DateTime(2018, 1, 5);

            //Act
            var date = await sut.GetDespatchDateAsync(new List<int>() { 1 }, orderDate);

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 8));
        }

        [Fact]
        public async Task LeadTimeNotCountedOverWeekend_SaturdayOrder()
        {
            // Arrange
            var sut = new DespatchService(_loggerMock.Object, _repositoryMock.Object);
            var supplierList = new List<Supplier> { SupplierWith1DayLeadTime };
            _repositoryMock.Setup(x => x.GetSuppliersListForProductList(It.IsAny<List<int>>())).ReturnsAsync(supplierList);
            var orderDate = new DateTime(2018, 1, 6);

            //Act
            var date = await sut.GetDespatchDateAsync(new List<int>() { 1 }, orderDate);

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 9));
        }

        [Fact]
        public async Task LeadTimeNotCountedOverWeekend_SundayOrder()
        {
            // Arrange
            var sut = new DespatchService(_loggerMock.Object, _repositoryMock.Object);
            var supplierList = new List<Supplier> { SupplierWith1DayLeadTime };
            _repositoryMock.Setup(x => x.GetSuppliersListForProductList(It.IsAny<List<int>>())).ReturnsAsync(supplierList);
            var orderDate = new DateTime(2018, 1, 7);

            //Act
            var date = await sut.GetDespatchDateAsync(new List<int>() { 1 }, orderDate);

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 9));
        }

        [Fact]
        public async Task LeadTimeOverMultipleWeeks_6Days()
        {
            // Arrange
            var sut = new DespatchService(_loggerMock.Object, _repositoryMock.Object);
            var supplierList = new List<Supplier> { SupplierWith6DaysLeadTime };
            _repositoryMock.Setup(x => x.GetSuppliersListForProductList(It.IsAny<List<int>>())).ReturnsAsync(supplierList);
            var orderDate = new DateTime(2018, 1, 5);

            //Act
            var date = await sut.GetDespatchDateAsync(new List<int>() { 1 }, orderDate);

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 15));
        }

        [Fact]
        public async Task LeadTimeOverMultipleWeeks_11Days()
        {
            // Arrange
            var sut = new DespatchService(_loggerMock.Object, _repositoryMock.Object);
            var supplierList = new List<Supplier> { SupplierWith11DaysLeadTime };
            _repositoryMock.Setup(x => x.GetSuppliersListForProductList(It.IsAny<List<int>>())).ReturnsAsync(supplierList);
            var orderDate = new DateTime(2018, 1, 5);

            //Act
            var date = await sut.GetDespatchDateAsync(new List<int>() { 1 }, orderDate);

            // Assert
            date.Date.ShouldBe(new DateTime(2018, 1, 22));
        }

        #endregion
    }
}