using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Moonpig.PostOffice.Data
{
    public class Repository : IRepository
    {
        private readonly IDbContext _dbContext;
        private readonly ILogger<Repository> _logger;

        #region Constructor
        public Repository(IDbContext dbContext, ILogger<Repository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        } 
        #endregion

        public async Task<Product> GetProduct(int productId)
        {
            return await Task.FromResult(_dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(productId)));
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersListForProductList(List<int> productIds)
        {
            var products = await Task.FromResult(_dbContext.Products.Where(p => productIds.Contains(p.ProductId)));

            if (products == null || !products.Any())
            {
                _logger.LogError("Unable to find product ids");
                throw new KeyNotFoundException(nameof(productIds));
            }

            var supplierIdsForProducts = products.Select(x => x.SupplierId);

            var suppliers = await Task.FromResult(_dbContext.Suppliers.Where(s => supplierIdsForProducts.Contains(s.SupplierId)));

            if (suppliers == null || !suppliers.Any())
            {
                _logger.LogError($"Unable to find suppliers for product list");
                throw new KeyNotFoundException(nameof(supplierIdsForProducts));
            }

            return suppliers;
        }
    }
}