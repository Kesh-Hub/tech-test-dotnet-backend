using System;
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

        public async Task<Supplier> GetSupplierForProduct(int productId)
        {
            var product = await Task.FromResult(_dbContext.Products.FirstOrDefault(p => p.ProductId.Equals(productId)));

            if (product == null)
            {
                _logger.LogError($"Unable to find supplier for product Id {productId}");
                throw new KeyNotFoundException(nameof(productId));
            }

            var supplier = await Task.FromResult(_dbContext.Suppliers.FirstOrDefault(s => s.SupplierId.Equals(product.SupplierId)));

            if (supplier == null)
            {
                _logger.LogError($"Unable to find supplier for supplier Id {product.SupplierId}");
                throw new KeyNotFoundException(nameof(product.SupplierId));
            }

            return supplier;
        }
    }
}