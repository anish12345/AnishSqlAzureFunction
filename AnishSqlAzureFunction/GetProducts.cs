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
using System.Collections.Generic;

namespace AnishSqlAzureFunction
{
    public static class GetProducts
    {
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            SqlConnection conn = GetConnection();
            try
            {
               
                List<Product> _productList = new List<Product>();
                string statment = "SELECT ProductID, ProductName, Quantity from Products";
                conn.Open();
                SqlCommand cmd = new SqlCommand(statment, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product()
                        {
                            ProductID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Quantity = reader.GetInt32(2)

                        };
                        _productList.Add(product);
                    }
                }
                conn.Close();
                return new OkObjectResult(_productList);
            }
            catch(Exception ex)
            {
                conn.Close();
                return new OkObjectResult(ex);
            }
        }
        private static SqlConnection GetConnection()
        {
            string conectionString = "Server=tcp:anishwebappserver.database.windows.net,1433;Initial Catalog=AnishWebAppDB;Persist Security Info=False;User ID=anish;Password=Indu@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return new SqlConnection(conectionString);
        }

        [FunctionName("GetProductByID")]
        public static async Task<IActionResult> RunProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            int productid = int.Parse(req.Query["id"]);
            string statment = String.Format("SELECT ProductID, ProductName, Quantity from Products WHERE ProductID = {0}", productid);
            SqlConnection conn = GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(statment, conn);
            Product product = new Product();
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    product.ProductID = reader.GetInt32(0);
                    product.ProductName = reader.GetString(1);
                    product.Quantity = reader.GetInt32(2);
                    var response = product;
                    conn.Close();
                    return new OkObjectResult(response);
                }
            }
            catch(Exception ex)
            {
                var response = "No Records";
                conn.Close();
                return new OkObjectResult(response);
            }
            conn.Close();
            
        }
    }
}
