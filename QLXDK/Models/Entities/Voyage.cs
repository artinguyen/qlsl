using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Entities
{
    public class Voyage
    {
        [Key]
        public int ID { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập tên kho")]
        public string Name { get; set; }
        public int SalanId { get; set; }
        public int DestinationPortId { get; set; }
    }
}