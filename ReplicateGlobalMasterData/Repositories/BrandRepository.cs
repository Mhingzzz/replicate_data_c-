using MySql.Data.MySqlClient;
using ReplicateGlobalMasterData.Entities;
using System.Data;
namespace ReplicateGlobalMasterData.Repositories
{


    namespace SkuSyncApp
    {
        public class BrandRepository
        {
            private readonly string prodDbConnectionString;
            private readonly string pilotDbConnectionString;

            public BrandRepository(string prodDbConnectionString, string pilotDbConnectionString)
            {
                this.prodDbConnectionString = prodDbConnectionString;
                this.pilotDbConnectionString = pilotDbConnectionString;
            }

            public List<brand> FetchAllBrandDataFromProdDB()
            {
                List<brand> brands = new List<brand>();

                using (MySqlConnection prodConn = new MySqlConnection(prodDbConnectionString))
                {
                    prodConn.Open();

                    string query = "SELECT BrandID, TH_Brand, EN_Brand FROM brand;";
                    using (MySqlCommand cmd = new MySqlCommand(query, prodConn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                brands.Add(new brand
                                {
                                    BrandID = reader.GetInt32("BrandID"),
                                    TH_Brand = reader.GetString("TH_Brand"),
                                    EN_Brand = reader.IsDBNull("EN_Brand") ? null : reader.GetString("EN_Brand"),
                                });
                            }
                        }
                    }

                    prodConn.Close();
                }

                return brands;
            }

            public void SyncBrandDataToPilotDB(List<brand> brandList)
            {
                using (MySqlConnection pilotConn = new MySqlConnection(pilotDbConnectionString))
                {
                    pilotConn.Open();

                    foreach (var brand in brandList)
                    {
                        string query = @"
                        INSERT INTO brand (BrandID, TH_Brand, EN_Brand)
                        VALUES (@BrandID, @TH_Brand, @EN_Brand)
                        ON DUPLICATE KEY UPDATE
                            TH_Brand = VALUES(TH_Brand),
                            EN_Brand = VALUES(EN_Brand);";

                        using (MySqlCommand cmd = new MySqlCommand(query, pilotConn))
                        {
                            cmd.Parameters.AddWithValue("@BrandID", brand.BrandID);
                            cmd.Parameters.AddWithValue("@TH_Brand", brand.TH_Brand);
                            cmd.Parameters.AddWithValue("@EN_Brand", brand.EN_Brand ?? (object)DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    pilotConn.Close();
                }

                Console.WriteLine("brand Data synchronization completed.");
            }
        }
    }

}

