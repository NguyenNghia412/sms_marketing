using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.GuiTinNhan
{
    [Table(nameof(BrandName), Schema = DbSchemas.Core)]
    public  class BrandName :ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdNhaCungCapDichVu { get; set; }
        public string TenBrandName { get; set; } = String.Empty;
        public string? Mota { get; set; } = String.Empty; 
        public DateTime? ThoiGianBatDauHoatDong {  get; set; }
        public DateTime? ThoiGianKetThucHoatDong { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
