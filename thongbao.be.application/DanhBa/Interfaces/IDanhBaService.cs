using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.shared.HttpRequest.BaseRequest;

namespace thongbao.be.application.DanhBa.Interfaces
{
    public interface  IDanhBaService
    {
        public void Create(CreateDanhBaDto dto);
        public void Update(int idDanhBa, UpdateDanhBaDto dto);
        public List<GetListDanhBaResponseDto> GetListDanhBa();
        public void CreateNguoiNhan( CreateNguoiNhanDto dto);
        public BaseResponsePagingDto<ViewDanhBaDto> Find(FindPagingDanhBaDto dto);
        public  Task<byte[]> ExportDanhBaChiTietExcelTemplate();
        public  Task<string> CreateDanhBaGoogleSheetTemplate();
        public Task<GetRefreshTokenDto> GetGoogleRefreshToken();
        public  Task<VerifyImportDanhBaCungResponseDto> VerifyImportAppendDanhBaCung(ImportAppendDanhBaCungDto dto);
        public Task<ImportDanhBaCungResponseDto> ImportAppendDanhBaCung(ImportAppendDanhBaCungDto dto);
    }
}
