using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace AnishSqlAzureFunction
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(requestBody);
            SqlConnection conn = GetConnection();
            conn.Open();
            string statement = "INSERT INTO Products(ProductID,ProductName,Quantity) VALUES(@param1,@param2,@param3)";
            using (SqlCommand command = new SqlCommand(statement, conn))
            {
                command.Parameters.Add("@param1", SqlDbType.Int).Value = product.ProductID;
                command.Parameters.Add("@param2", SqlDbType.VarChar, 1000).Value = product.ProductName;
                command.Parameters.Add("@param3", SqlDbType.Decimal).Value = product.Quantity;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

            }

            return new OkObjectResult("Product added");
        }

        private static SqlConnection GetConnection()
        {
            string conectionString = "Server=tcp:anishwebappserver.database.windows.net,1433;Initial Catalog=AnishWebAppDB;Persist Security Info=False;User ID=anish;Password=Indu@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return new SqlConnection(conectionString);
        }
    }
}
