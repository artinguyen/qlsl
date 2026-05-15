using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Entities
{
    public class Customer
    {
        [Key]
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}