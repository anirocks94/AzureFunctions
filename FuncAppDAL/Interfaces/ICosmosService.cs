using FuncAppDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncAppDAL.Interfaces
{
    public interface ICosmosService
    {
        public bool AddOrUpdateCustomerDetailsIntoDb(Customer cust);
    }
}
