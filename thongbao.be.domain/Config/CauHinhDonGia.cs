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

namespace thongbao.be.domain.Config
{
    [Table(nameof(CauHinhDonGia), Schema = DbSchemas.Core)]
    [Index(
     nameof(Id),
     IsUnique = false,
     Name = $"IX_{nameof(CauHinhDonGia)}"
   )]
    public class CauHinhDonGia : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdNhaCungCapDichVu { get; set; }
        public int IdBrandName { get; set; }
        public int IdNhaMang {  get; set; } 
        public int DonGia {  get; set; }
        public DateTime? ThoiHan {  get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
