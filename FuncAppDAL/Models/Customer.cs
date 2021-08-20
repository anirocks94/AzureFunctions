using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncAppDAL.Models
{
    [BsonIgnoreExtraElements]
    public class Customer
    {
        [BsonRepresentation(BsonType.ObjectId)]
        
        public string Id { get; set; }
        public int pk { get; set; }
        public string FirstName { get; set; }
        

        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public dynamic Address { get; set; }
        public dynamic Age { get; set; }
        public dynamic PhoneNumber { get; set; }
        public dynamic CustomerProfileType { get; set; }
        public dynamic DateOfBirth { get; set; }



    }
}
