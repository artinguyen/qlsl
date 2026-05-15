using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Views
{
    public class PortOfLoadingVM
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập cảng")]
        [Display(Name = "Tên cảng")]
        public string PortCode { get; set; }
        public string PortName { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }

        //public System.DateTime? CreatedAt { get; set; }
        //public System.DateTime? UpdatedAt { get; set; }
    }
}