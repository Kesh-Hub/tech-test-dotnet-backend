using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moonpig.PostOffice.Api.Model;
using Moonpig.PostOffice.Data;

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
            var maxLeadTime = await GetMaxLeadTime(productIds, orderDate);

            switch (maxLeadTime.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return new DespatchDate {Date = maxLeadTime.AddDays(2)};
                case DayOfWeek.Sunday:
                    return new DespatchDate {Date = maxLeadTime.AddDays(1)};
                default:
                    return new DespatchDate {Date = maxLeadTime};
            }
        }

        #region Helper Functions

        private async Task<DateTime> GetMaxLeadTime(List<int> productIds, DateTime orderDate)
        {
            var maxLeadTime = orderDate;

            foreach (var productId in productIds)
            {
                // Retrieve the lead time for each product
                var supplier = await _repository.GetSupplierForProduct(productId);
                var leadTime = supplier.LeadTime;

                // Calculate the max lead time
                if (orderDate.AddDays(leadTime) > maxLeadTime)
                    maxLeadTime = orderDate.AddDays(leadTime);
            }

            return maxLeadTime;
        }

        #endregion

    }
}