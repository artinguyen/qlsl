using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLXDK.Models.Entities
{
    public class SubVoyage
    {
        [Key]
        public int ID { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập tên kho")]
        public int VoyageId { get; set; }
        [ForeignKey("VoyageId")]
        public virtual Voyage Voyage { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn cảng đi")]
        public int PortOfLoadingId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn cảng đến")]
        public int DestinationPortId { get; set; }
        public System.DateTime? CreatedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
    }
}