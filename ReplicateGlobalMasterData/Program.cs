using ReplicateGlobalMasterData.Entities;
using ReplicateGlobalMasterData.Repositories;

namespace SkuSyncApp
{


    class Program
    {
        private static readonly string prodDbConnectionString = "server=localhost;port=3306;userid=user;password=pass;database=exampledb;";
        private static readonly string pilotDbConnectionString = "server=localhost;port=3308;userid=user;password=pass;database=db_pilot;";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting data synchronization...");

            // Sync SKU Data
            SkuRepository skuRepo = new SkuRepository(prodDbConnectionString, pilotDbConnectionString);
            List<sku> skus = skuRepo.FetchAllSkuDataFromProdDB();
            //skuRepo.SyncSkuDataToPilotDB(skus);
            string filePath = @"C:\TCC\C#\ReplicateGlobalMasterData\ReplicateGlobalMasterData\Json\skuData.json";

            // Export the SKU data to JSON
            skuRepo.ExportSkuDataToJson(skus, filePath);


            // Sync Brand Data
            //BrandRepository brandRepo = new BrandRepository(prodDbConnectionString, pilotDbConnectionString);
            //List<brand> brands = brandRepo.FetchAllBrandDataFromProdDB();
            //brandRepo.SyncBrandDataToPilotDB(brands);

            Console.WriteLine("Data synchronization completed.");
        }
    }
}
