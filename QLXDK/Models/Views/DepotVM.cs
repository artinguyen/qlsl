using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Views
{
    public class DepotVM
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên depot")]
        [Display(Name = "Tên depot")]
        public string Name { get; set; }
        public string Address { get; set; }
    }
}