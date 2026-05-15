using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Views
{
    public class SalanVM
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên sà lan")]
        [Display(Name = "Sà lan")]
        public string Name { get; set; }
        public int Teus { get; set; }
    }
}