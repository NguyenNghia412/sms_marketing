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

namespace thongbao.be.domain.TraoBang
{
    [Table(nameof(SubPlan), Schema = DbSchemas.TraoBang)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(SubPlan)}"
  )]
    public class SubPlan : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdPlan { get; set; }
        public string Ten { get; set; } = String.Empty;
        public string? MoTa { get; set; } = String.Empty;
        public string? Note { get; set; } = String.Empty;
        public string MoBai { get; set; } = String.Empty;
        public string KetBai { get; set; } = String.Empty;
        public int Order { get; set; }
        public bool IsShow { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}
