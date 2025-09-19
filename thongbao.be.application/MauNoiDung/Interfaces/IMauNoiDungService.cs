using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.MauNoiDung.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.MauNoiDung.Interfaces
{
    public interface  IMauNoiDungService
    {
        public void Create(CreateMauNoiDungDto dto);
        public void Update(int id, UpdateMauNoiDungDto dto);
        public BaseResponsePagingDto<ViewMauNoiDungDto> Find(FindPagingMauNoiDungDto dto);
        public void Delete(int id);
        public void CreateChienDichByMauNoiDung(int id, CreateChienDichByMauNoiDungDto dto);
    }
}
