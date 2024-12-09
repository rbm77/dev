using System.Data.Common;
using Pos.Interfaces;
using Pos.Models;

namespace Pos.Services
{
    public class ProductService(IDatabase database) : IProductService
    {

        public async Task<List<Product>> GetAllProducts()
        {
            const string procedureName = "GetAllProducts";

            return await database.ExecuteReader(procedureName, reader =>
            {
                List<Product> products = [];

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = reader["ProductId"].ToString(),
                        TenantId = reader["TenantId"].ToString(),
                        SKU = reader["SKU"].ToString(),
                        Barcode = reader["Barcode"].ToString(),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Stock = Convert.ToInt32(reader["Stock"]),
                        MinimumStock = Convert.ToInt32(reader["MinimumStock"]),
                        CategoryId = reader["CategoryId"].ToString(),
                        UnitId = reader["UnitId"].ToString(),
                        SupplierId = reader["SupplierId"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }

                return products;
            }, []);
        }

        public async Task<Product?> GetProductById(string productId)
        {
            const string procedureName = "GetProductById";

            DbParameter[] parameters =
            [
                database.CreateParameter("@ProductId", productId)
            ];

            return await database.ExecuteReader(procedureName, reader =>
            {
                if (reader.Read())
                {
                    return new Product
                    {
                        ProductId = reader["ProductId"].ToString(),
                        TenantId = reader["TenantId"].ToString(),
                        SKU = reader["SKU"].ToString(),
                        Barcode = reader["Barcode"].ToString(),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Stock = Convert.ToInt32(reader["Stock"]),
                        MinimumStock = Convert.ToInt32(reader["MinimumStock"]),
                        CategoryId = reader["CategoryId"].ToString(),
                        UnitId = reader["UnitId"].ToString(),
                        SupplierId = reader["SupplierId"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    };
                }
                return null;
            }, parameters);
        }

        public async Task<string> CreateProduct(Product product)
        {
            const string procedureName = "CreateProduct";

            var parameters = new[]
            {
                database.CreateParameter("@TenantId", product.TenantId),
                database.CreateParameter("@SKU", product.SKU),
                database.CreateParameter("@Barcode", product.Barcode),
                database.CreateParameter("@Name", product.Name),
                database.CreateParameter("@Description", product.Description),
                database.CreateParameter("@Price", product.Price),
                database.CreateParameter("@Stock", product.Stock),
                database.CreateParameter("@MinimumStock", product.MinimumStock),
                database.CreateParameter("@CategoryId", product.CategoryId),
                database.CreateParameter("@UnitId", product.UnitId),
                database.CreateParameter("@SupplierId", product.SupplierId),
                database.CreateParameter("@IsActive", product.IsActive)
            };

            object? createdProductId = await database.ExecuteScalar(procedureName, parameters);

            return createdProductId?.ToString() ?? "";
        }

        public async Task<bool> UpdateProduct(string productId, Product product)
        {
            const string procedureName = "UpdateProduct";

            DbParameter[] parameters =
            [
                database.CreateParameter("@ProductId", productId),
                database.CreateParameter("@TenantId", product.TenantId),
                database.CreateParameter("@SKU", product.SKU),
                database.CreateParameter("@Barcode", product.Barcode),
                database.CreateParameter("@Name", product.Name),
                database.CreateParameter("@Description", product.Description),
                database.CreateParameter("@Price", product.Price),
                database.CreateParameter("@Stock", product.Stock),
                database.CreateParameter("@MinimumStock", product.MinimumStock),
                database.CreateParameter("@CategoryId", product.CategoryId),
                database.CreateParameter("@UnitId", product.UnitId),
                database.CreateParameter("@SupplierId", product.SupplierId),
                database.CreateParameter("@IsActive", product.IsActive)
            ];

            int rowsAffected = await database.ExecuteNonQuery(procedureName, parameters);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteProduct(string productId)
        {
            const string procedureName = "DeleteProduct";

            DbParameter[] parameters =
            [
                database.CreateParameter("@ProductId", productId)
            ];

            int rowsAffected = await database.ExecuteNonQuery(procedureName, parameters);

            return rowsAffected > 0;
        }
    }
}
