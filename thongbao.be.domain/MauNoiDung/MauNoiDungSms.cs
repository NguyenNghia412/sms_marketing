using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using thongbao.be.domain.GuiTinNhan;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.MauNoiDung
{
    [Table(nameof(MauNoiDungSms), Schema = DbSchemas.Core)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(MauNoiDungSms)}"
    )]
    public class MauNoiDungSms :ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TenMauNoiDung { get; set; } = String.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }

    }
}
