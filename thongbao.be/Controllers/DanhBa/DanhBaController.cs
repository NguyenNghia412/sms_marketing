using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using thongbao.be.application.DanhBa.Dtos;
using thongbao.be.application.DanhBa.Interfaces;
using thongbao.be.Attributes;
using thongbao.be.Controllers.Auth;
using thongbao.be.Controllers.Base;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest;


namespace thongbao.be.Controllers.DanhBa
{
    [Route("api/core/danh-ba")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DanhBaController : BaseController
    {
        private readonly IDanhBaService _danhBaService;
        private readonly ILogger<DanhBaController> _logger;
        public DanhBaController(ILogger<DanhBaController> logger, IDanhBaService danhBaService) : base(logger)
        {
            _danhBaService = danhBaService;
            _logger = logger;
        }

        [Permission(PermissionKeys.DanhBaAdd)]
        [HttpPost("")]
        public ApiResponse Create([FromBody] CreateDanhBaDto dto)
        {
            try
            {
                _danhBaService.Create(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaAdd)]
        [HttpPost("nguoi-nhan-moi")]
        public ApiResponse CreateNguoiNhan(CreateNguoiNhanDto dto)
        {
            try
            {
                _danhBaService.CreateNguoiNhan(dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaUpdate)]
        [HttpPut("")]
        public ApiResponse Update([FromQuery] int idDanhBa, [FromBody] UpdateDanhBaDto dto)
        {
            try
            {
                _danhBaService.Update(idDanhBa, dto);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaView)]
        [HttpGet("")]
        public ApiResponse Find([FromQuery] FindPagingDanhBaDto dto)
        {
            try
            {
                var result = _danhBaService.Find(dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaView)]
        [HttpGet("paging-danh-ba-chi-tiet")]
        public ApiResponse FindDanhBaChiTiet([FromQuery] int idDanhBa, [FromQuery] FindPagingDanhBaChiTietDto dto)
        {
            try
            {
                var result = _danhBaService.FindDanhBaChiTiet(idDanhBa, dto);
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaView)]
        [HttpGet("list-danh-ba")]
        public ApiResponse GetListDanhBa()
        {
            try
            {
                var result = _danhBaService.GetListDanhBa();
                return new(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaView)]
        [HttpPost("export-danh-ba-cung-template-excel")]
        public async Task<IActionResult> DownloadDanhBaCungTemplateExcel()
        {
            try
            {
                var excelTemplate = await _danhBaService.ExportDanhBaCungExcelTemplate();
                var fileName = $"Mẫu File Excel Import Danh Bạ Người Dùng.xlsx";
                return File(
                   excelTemplate,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                 );

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(ex.Message));
            }
        }
        [Permission(PermissionKeys.DanhBaView)]
        [HttpPost("export-danh-ba-chi-tiet--template-excel")]
        public async Task<IActionResult> DownloadDanhBaChiTietTemplateExcel()
        {
            try
            {
                var excelTemplate = await _danhBaService.ExportDanhBaChiTietExcelTemplate();
                var fileName = $"Mẫu File Excel Import Danh Bạ Người Dùng Theo Chiến Dịch.xlsx";
                return File(
                   excelTemplate,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                 );

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(ex.Message));
            }
        }
        /*[Permission(PermissionKeys.DanhBaView)]
        [HttpPost("danh-ba-template-google-sheet")]
        public async Task<ApiResponse> CreateDanhBaGoogleSheetTemplate()
        {
            try
            {
                var data = await _danhBaService.CreateDanhBaGoogleSheetTemplate();
                return new(data);
            }catch(Exception ex)
            {
                return OkException(ex);
            }
        }
        [Permission(PermissionKeys.DanhBaView)]
        [HttpPost("get-google-refresh-token")]
        public async Task<ApiResponse> GetGoogleRefreshToken()
        {
            try
            {
                var data = await _danhBaService.GetGoogleRefreshToken();
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }*/

        [Permission(PermissionKeys.DanhBaImport)]
        [HttpPost("verify-import-google-sheet-append")]
        public async Task<ApiResponse> VerifyImportAppendDanhBaCung([FromBody] ImportAppendDanhBaCungDto dto)
        {
            try
            {
                var data = await _danhBaService.VerifyImportAppendDanhBaCung(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.DanhBaImport)]
        [HttpPost("verify-import-danh-ba-chien-dich")]
        public async Task<ApiResponse> VerifyImportDanhBaChienDich([FromForm] ImportAppendDanhBaChienDichDto dto)
        {
            try
            {
                var data = await _danhBaService.VerifyImportDanhBaChienDich(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.DanhBaImport)]
        [HttpPost("import-google-sheet-append")]
        public async Task<ApiResponse> ImportAppendDanhBaCung([FromBody] ImportAppendDanhBaCungDto dto)
        {
            try
            {
                var data = await _danhBaService.ImportAppendDanhBaCung(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [Permission(PermissionKeys.DanhBaImport)]
        [HttpPost("import-danh-ba-chien-dich-append")]
        public async Task<ApiResponse> ImportAppendDanhBaChienDich([FromForm] ImportAppendDanhBaChienDichDto dto)
        {
            try
            {
                var data = await _danhBaService.ImportAppendDanhBaChienDich(dto);
                return new(data);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }




    }
}