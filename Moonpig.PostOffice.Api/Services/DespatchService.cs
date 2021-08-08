using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moonpig.PostOffice.Api.Model;
using Moonpig.PostOffice.Data;
using DayOfWeek = System.DayOfWeek;

namespace Moonpig.PostOffice.Api.Services
{
    public class DespatchService : IDespatchService
    {
        private readonly ILogger<DespatchService> _logger;
        private readonly IRepository _repository;

        #region Constructor
        public DespatchService(ILogger<DespatchService> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        #endregion

        public async Task<DespatchDate> GetDespatchDateAsync(List<int> productIds, DateTime orderDate)
        {
            // Find unique products only to reduce number of DB calls
            var distinctProductIds = productIds.Distinct().ToList();

            // Retrieve maximum lead time from suppliers
            var maxLeadTime = await GetMaxLeadIncludingWeekendsAsync(distinctProductIds, orderDate);

            // Check for the next post office working day
            var postedDate = GetNextWorkingDay(maxLeadTime);

            return new DespatchDate {Date = postedDate};
        }

        #region Helper Functions

        private static DateTime GetNextWorkingDay(DateTime workingDate)
        {
            switch (workingDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return workingDate.AddDays(2);
                case DayOfWeek.Sunday:
                    return workingDate.AddDays(1);
                default:
                    return workingDate;
            }
        }

        private async Task<DateTime> GetMaxLeadIncludingWeekendsAsync(List<int> productIds, DateTime orderDate)
        {
            // Get the next working day for the supplier to process the order
            var supplierProcessingDate = GetNextWorkingDay(orderDate);

            var suppliersForProducts = await _repository.GetSuppliersListForProductList(productIds);

            var maxLeadTime = suppliersForProducts.Max(x => x.LeadTime);
            
            // return the processed date including the lead time
            return supplierProcessingDate.AddWorkingDays(maxLeadTime);
        }

        #endregion

    }

    #region Extensions

    public static class DateTimeExtensions
    {
        public static DateTime AddWorkingDays(this DateTime dateTime, double daysToAdd)
        {
            var result = dateTime;
            while (daysToAdd > 0)
            {
                result = result.AddDays(1);

                // Skip weekend
                if (result.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    continue;
                }

                daysToAdd--;
            }

            return result;
        }
    }

    #endregion
}