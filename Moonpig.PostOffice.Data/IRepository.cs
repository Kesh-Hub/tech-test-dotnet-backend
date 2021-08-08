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
        /// Returns the Supplier details for a product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<Supplier> GetSupplierForProduct(int productId);
    }
}