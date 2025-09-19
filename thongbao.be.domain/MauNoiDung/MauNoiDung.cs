using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.MauNoiDung
{
    [Table(nameof(MauNoiDung), Schema = DbSchemas.Core)]
    public class MauNoiDung :ISoftDelted
    {
        public int Id { get; set; }
        public string TenMauNoiDung { get; set; } = String.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }

    }
}
