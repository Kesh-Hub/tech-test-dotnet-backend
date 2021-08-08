using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonpig.PostOffice.Data
{
    public interface IRepository
    {
        /// <summary>
        /// Returns Product from product Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<Product> GetProduct(int productId);
        
        /// <summary>
        /// Returns the list of Supplier for the list of products
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<IEnumerable<Supplier>> GetSuppliersListForProductList(List<int> productIds);
    }
}