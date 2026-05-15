using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Views
{
    public class UnloadingLocationVM
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nơi hạ")]
        [Display(Name = "Nơi hạ")]
        public string Name { get; set; }
        public string Address { get; set; }
    }
}