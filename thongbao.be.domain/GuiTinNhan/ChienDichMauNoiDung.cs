using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.shared.Constants.Db;
using thongbao.be.shared.Interfaces;

namespace thongbao.be.domain.GuiTinNhan
{
    [Table(nameof(ChienDichMauNoiDung), Schema = DbSchemas.Core)]
    public class ChienDichMauNoiDung:ISoftDelted
    {
        public int IdChienDich { get; set; }
        public int IdMauNoiDung { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }
}
