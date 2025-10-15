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
    [Table(nameof(TraoBangLog), Schema = DbSchemas.TraoBang)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(TraoBangLog)}"
  )]
    public class TraoBangLog : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdSubPlan { get; set; }
        public int IdSinhVien   { get; set; }
        public string NoiDung { get; set; } = String.Empty;
        public string? Note { get; set; } = String.Empty;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
