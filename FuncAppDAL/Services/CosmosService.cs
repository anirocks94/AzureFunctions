using FuncAppDAL.DBContext;
using FuncAppDAL.Interfaces;
using FuncAppDAL.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FuncAppDAL.Services
{
    public class CosmosService : ICosmosService
    {
        public  CosmosDbContext _mongocontext;
        public CosmosService()
        {
            _mongocontext = new CosmosDbContext();
        }

        public bool AddOrUpdateCustomerDetailsIntoDb(Customer cust)
        {
            try
            {   ///Insert Emoloyeee  

                if (cust.Id == null)
                {
                    var collection = _mongocontext.GetCustomers;
                    collection.InsertOne(cust);

                    return true;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
