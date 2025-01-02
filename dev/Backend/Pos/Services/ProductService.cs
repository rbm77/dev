using System.Data.Common;
using Pos.Interfaces;
using Pos.Models;
using static Pos.Utilities.Enums;

namespace Pos.Services
{
    public class ProductService(IDatabase database, ILogManager logManager) : IProductService
    {

        public async Task<List<Product>> GetAllProducts()
        {
            try
            {
                return await database.ExecuteReader("GetAllProducts", reader =>
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
                            CategoryId = reader["CategoryId"].ToString(),
                            UnitId = reader["UnitId"].ToString(),
                            TaxId = reader["TaxId"].ToString(),
                            SupplierId = reader["SupplierId"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }

                    return products;
                }, []);
            }
            catch (Exception ex)
            {
                _ = logManager.Log(ex.Message, LogType.Error);
                throw;
            }
        }

        public async Task<Product?> GetProductById(string productId)
        {
            try
            {
                DbParameter[] parameters =
                [
                    database.CreateParameter("@ProductId", productId)
                ];

                return await database.ExecuteReader("GetProductById", reader =>
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
                            CategoryId = reader["CategoryId"].ToString(),
                            UnitId = reader["UnitId"].ToString(),
                            TaxId = reader["TaxId"].ToString(),
                            SupplierId = reader["SupplierId"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                    return null;
                }, parameters);
            }
            catch (Exception ex)
            {
                _ = logManager.Log(ex.Message, LogType.Error);
                throw;
            }
        }

        public async Task<string> CreateProduct(Product product)
        {
            try
            {
                DbParameter[] parameters =
                [
                    database.CreateParameter("@TenantId", product.TenantId),
                    database.CreateParameter("@SKU", product.SKU),
                    database.CreateParameter("@Barcode", product.Barcode),
                    database.CreateParameter("@Name", product.Name),
                    database.CreateParameter("@Description", product.Description),
                    database.CreateParameter("@Price", product.Price),
                    database.CreateParameter("@CategoryId", product.CategoryId),
                    database.CreateParameter("@UnitId", product.UnitId),
                    database.CreateParameter("@TaxId", product.TaxId),
                    database.CreateParameter("@SupplierId", product.SupplierId),
                    database.CreateParameter("@IsActive", product.IsActive)
                ];

                object? createdProductId = await database.ExecuteScalar("CreateProduct", parameters);

                return createdProductId?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                _ = logManager.Log(ex.Message, LogType.Error);
                throw;
            }
        }

        public async Task<bool> UpdateProduct(string productId, Product product)
        {
            try
            {
                DbParameter[] parameters =
                [
                    database.CreateParameter("@ProductId", productId),
                    database.CreateParameter("@TenantId", product.TenantId),
                    database.CreateParameter("@SKU", product.SKU),
                    database.CreateParameter("@Barcode", product.Barcode),
                    database.CreateParameter("@Name", product.Name),
                    database.CreateParameter("@Description", product.Description),
                    database.CreateParameter("@Price", product.Price),
                    database.CreateParameter("@CategoryId", product.CategoryId),
                    database.CreateParameter("@UnitId", product.UnitId),
                    database.CreateParameter("@TaxId", product.TaxId),
                    database.CreateParameter("@SupplierId", product.SupplierId),
                    database.CreateParameter("@IsActive", product.IsActive)
                ];

                int rowsAffected = await database.ExecuteNonQuery("UpdateProduct", parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _ = logManager.Log(ex.Message, LogType.Error);
                throw;
            }
        }

        public async Task<bool> DeleteProduct(string productId)
        {
            try
            {
                DbParameter[] parameters =
                [
                    database.CreateParameter("@ProductId", productId)
                ];

                int rowsAffected = await database.ExecuteNonQuery("DeleteProduct", parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _ = logManager.Log(ex.Message, LogType.Error);
                throw;
            }
        }
    }
}
