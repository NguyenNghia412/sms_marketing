using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.ToChuc
{
    [Table(nameof(ToChuc), Schema = DbSchemas.Core)]
    [Index(
     nameof(Id),
     IsUnique = false,
     Name = $"IX_{nameof(ToChuc)}"
    )]
    public class ToChuc : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TenToChuc { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public int  LoaiToChuc { get; set; }  
        public string? MaSoToChuc { get; set; } = String.Empty;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }

    }
}
