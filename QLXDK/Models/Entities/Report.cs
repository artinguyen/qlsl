using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QLXDK.Models.Entities
{
    public class Report
    {
        [Required(ErrorMessage = "Vui lòng nhập Line")]
        public string Line { get; set; }
        public string Vessel { get; set; }
        public string ICDs { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public System.DateTime? FromDate { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public System.DateTime? ToDate { get; set; }
    }
}