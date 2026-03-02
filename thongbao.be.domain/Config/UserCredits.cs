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
    [Table(nameof(UserCredits), Schema = DbSchemas.Core)]
    [Index(
     nameof(Id),
     IsUnique = false,
     Name = $"IX_{nameof(UserCredits)}"
   )]
    public class UserCredits : ISoftDelted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; } = String.Empty;
        public int IdNhaCungCapDichVu { get; set; } 
        public string HanMucCredit {  get; set; } = String.Empty ;
        //public string? ToiDaHanMucCreditGiaHan {  get; set; } = String.Empty;
        public DateTime ThoiGianBatDauApDungHanMuc {  get; set; }
        public DateTime ThoiGianKetThucApDungHanMuc { get; set; }
        public string? CreditDaSuDung { get; set; }= String.Empty;
        public string? CreditChuaSuDung { get; set; } = String.Empty;
        public string? CreditConSauKhiKetThucThoiGianApDungHanMuc { get; set; } = String.Empty;
        public string DonVi { get; set; } = "VND";

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
