using MySql.Data.MySqlClient;
using ReplicateGlobalMasterData.Entities;
using System.Data;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ReplicateGlobalMasterData.Repositories
{
    public class SkuRepository
    {
        private readonly string prodDbConnectionString;
        private readonly string pilotDbConnectionString;

        public SkuRepository(string prodDbConnectionString, string pilotDbConnectionString)
        {
            this.prodDbConnectionString = prodDbConnectionString;
            this.pilotDbConnectionString = pilotDbConnectionString;
        }

        public List<sku> FetchAllSkuDataFromProdDB()
        {
            List<sku> skus = new List<sku>();

            using (MySqlConnection prodConn = new MySqlConnection(prodDbConnectionString))
            {
                prodConn.Open();

                string query = "SELECT SKUID, BarcodePOS, ProductName, BrandID, ProductGroupID, ProductCatID, ProductSubCatID, ProductSizeID, ProductUnit, PackSize, Unit, BanForPracharat, IsVat, CreateBy, CreateDate, IsActive, MerchantID, MapSKU, IsFixPrice, UpdateBy, UpdateDate, IsApprove, ApproveBy, ApproveDate FROM sku WHERE MerchantID IS NULL;";
                using (MySqlCommand cmd = new MySqlCommand(query, prodConn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            skus.Add(new sku
                            {
                                SKUID = reader.GetString("SKUID"),
                                BarcodePOS = reader.GetString("BarcodePOS"),
                                ProductName = reader.IsDBNull("ProductName") ? null : reader.GetString("ProductName"),
                                BrandID = reader.IsDBNull("BrandID") ? null : reader.GetInt32("BrandID"),
                                ProductGroupID = reader.IsDBNull("ProductGroupID") ? null : reader.GetInt32("ProductGroupID"),
                                ProductCatID = reader.IsDBNull("ProductCatID") ? null : reader.GetInt32("ProductCatID"),
                                ProductSubCatID = reader.IsDBNull("ProductSubCatID") ? null : reader.GetInt32("ProductSubCatID"),
                                ProductSizeID = reader.IsDBNull("ProductSizeID") ? null : reader.GetInt32("ProductSizeID"),
                                ProductUnit = reader.IsDBNull("ProductUnit") ? null : reader.GetInt32("ProductUnit"),
                                PackSize = reader.IsDBNull("PackSize") ? null : reader.GetString("PackSize"),
                                Unit = reader.IsDBNull("Unit") ? null : reader.GetInt32("Unit"),
                                BanForPracharat = reader.IsDBNull("BanForPracharat") ? null : reader.GetInt32("BanForPracharat"),
                                IsVat = reader.IsDBNull("IsVat") ? null : reader.GetBoolean("IsVat"),
                                CreateBy = reader.GetString("CreateBy"),
                                CreateDate = reader.GetDateTime("CreateDate"),
                                IsActive = reader.GetBoolean("IsActive"),
                                MerchantID = reader.IsDBNull("MerchantID") ? null : reader.GetString("MerchantID"),
                                MapSKU = reader.IsDBNull("MapSKU") ? null : reader.GetString("MapSKU"),
                                IsFixPrice = reader.GetBoolean("IsFixPrice"),
                                UpdateBy = reader.IsDBNull("UpdateBy") ? null : reader.GetString("UpdateBy"),
                                UpdateDate = reader.IsDBNull("UpdateDate") ? null : reader.GetDateTime("UpdateDate"),
                                IsApprove = reader.IsDBNull("IsApprove") ? null : reader.GetBoolean("IsApprove"),
                                ApproveBy = reader.IsDBNull("ApproveBy") ? null : reader.GetString("ApproveBy"),
                                ApproveDate = reader.IsDBNull("ApproveDate") ? null : reader.GetDateTime("ApproveDate")
                            });
                        }
                    }
                }

                prodConn.Close();
            }

            return skus;
        }

        public void SyncSkuDataToPilotDB(List<sku> skuList)
        {
            using (MySqlConnection pilotConn = new MySqlConnection(pilotDbConnectionString))
            {
                pilotConn.Open();

                foreach (var sku in skuList)
                {
                    string query = @"
                INSERT INTO sku (SKUID, BarcodePOS, ProductName, BrandID, ProductGroupID, ProductCatID, 
                                 ProductSubCatID, ProductSizeID, ProductUnit, PackSize, Unit, BanForPracharat, 
                                 IsVat, CreateBy, CreateDate, IsActive, MerchantID, MapSKU, IsFixPrice, UpdateBy, 
                                 UpdateDate, IsApprove, ApproveBy, ApproveDate)
                VALUES (@SKUID, @BarcodePOS, @ProductName, @BrandID, @ProductGroupID, @ProductCatID, @ProductSubCatID, 
                        @ProductSizeID, @ProductUnit, @PackSize, @Unit, @BanForPracharat, @IsVat, @CreateBy, @CreateDate, 
                        @IsActive, @MerchantID, @MapSKU, @IsFixPrice, @UpdateBy, @UpdateDate, @IsApprove, @ApproveBy, @ApproveDate)
                ON DUPLICATE KEY UPDATE
                    BarcodePOS = VALUES(BarcodePOS),
                    ProductName = VALUES(ProductName),
                    BrandID = VALUES(BrandID),
                    ProductGroupID = VALUES(ProductGroupID),
                    ProductCatID = VALUES(ProductCatID),
                    ProductSubCatID = VALUES(ProductSubCatID),
                    ProductSizeID = VALUES(ProductSizeID),
                    ProductUnit = VALUES(ProductUnit),
                    PackSize = VALUES(PackSize),
                    Unit = VALUES(Unit),
                    BanForPracharat = VALUES(BanForPracharat),
                    IsVat = VALUES(IsVat),
                    CreateBy = VALUES(CreateBy),
                    CreateDate = VALUES(CreateDate),
                    IsActive = VALUES(IsActive),
                    MerchantID = VALUES(MerchantID),
                    MapSKU = VALUES(MapSKU),
                    IsFixPrice = VALUES(IsFixPrice),
                    UpdateBy = VALUES(UpdateBy),
                    UpdateDate = VALUES(UpdateDate),
                    IsApprove = VALUES(IsApprove),
                    ApproveBy = VALUES(ApproveBy),
                    ApproveDate = VALUES(ApproveDate);";

                    using (MySqlCommand cmd = new MySqlCommand(query, pilotConn))
                    {
                        // Adding parameters
                        cmd.Parameters.AddWithValue("@SKUID", sku.SKUID);
                        cmd.Parameters.AddWithValue("@BarcodePOS", sku.BarcodePOS);
                        cmd.Parameters.AddWithValue("@ProductName", sku.ProductName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BrandID", sku.BrandID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductGroupID", sku.ProductGroupID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductCatID", sku.ProductCatID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductSubCatID", sku.ProductSubCatID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductSizeID", sku.ProductSizeID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductUnit", sku.ProductUnit ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PackSize", sku.PackSize ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Unit", sku.Unit ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BanForPracharat", sku.BanForPracharat ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsVat", sku.IsVat ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreateBy", sku.CreateBy);
                        cmd.Parameters.AddWithValue("@CreateDate", sku.CreateDate);
                        cmd.Parameters.AddWithValue("@IsActive", sku.IsActive);
                        cmd.Parameters.AddWithValue("@MerchantID", sku.MerchantID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MapSKU", sku.MapSKU ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsFixPrice", sku.IsFixPrice);
                        cmd.Parameters.AddWithValue("@UpdateBy", sku.UpdateBy ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@UpdateDate", sku.UpdateDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsApprove", sku.IsApprove ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ApproveBy", sku.ApproveBy ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ApproveDate", sku.ApproveDate ?? (object)DBNull.Value);

                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }
                }

                pilotConn.Close();
            }

            Console.WriteLine("SKU Data synchronization completed.");
        }


        public void ExportSkuDataToJson(List<sku> skuList, string filePath)
        {
            // Get the directory from the file path
            string directory = Path.GetDirectoryName(filePath);

            // Check if the directory exists, if not, create it
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Configure JsonSerializer options to prevent escaping of non-ASCII characters
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Pretty print (indent) the JSON
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Prevents escaping of non-ASCII characters
            };

            // Serialize the list of SKUs to JSON using System.Text.Json
            string json = JsonSerializer.Serialize(skuList, options);

            // Write the JSON string to a file
            File.WriteAllText(filePath, json);

            Console.WriteLine($"SKU data has been exported to {filePath}");
        }
    }
}
