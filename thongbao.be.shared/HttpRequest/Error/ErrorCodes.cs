using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.shared.HttpRequest.Error
{
    public static class ErrorCodes
    {
        //Các mã lỗi căn bản
        public const int System = 1;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int NotFound = 404;
        public const int Found = 409;
        public const int InternalServerError = 500;

        public const int AuthInvalidPassword = 101;
        public const int AuthErrorCreateUser = 102;
        public const int AuthErrorUserNotFound = 103;
        public const int AuthErrorCreateRole = 104;
        public const int AuthErrorRoleNotFound = 105;
        public const int AuthErrorUserEmailHuceNotFound = 106;

        public const int CuocHopErrorNotFound = 201;
        public const int CuocHopErrorNameFound = 202;
        public const int CuocHopErrorTime = 203;
        public const int CuocHopErrorTimeDotDiemDanh = 204;
        public const int CuocHopErrorInvalidJoinWebUrl = 205;
        public const int CuocHopErrorMeetingNotFound = 206;
        public const int CuocHopErrorDotDiemDanhNotFound = 207;
        public const int CuocHopErrorDaDiemDanh = 208;


        public const int ChienDichErrorNotFound = 301;
        public const int MauNoiDungErrorNotFound = 302;
        public const int ChienDichErrorBrandNameNotFound = 303;
        public const int ChienDichErrorTrangThaiTrue = 304;
        public const int ChienDichErrorTrangThaiTrueCannotDelete = 305;
       

        public const int DanhBaErrorNotFound = 501;
        public const int DanhBaErrorRequired = 502;
        public const int DanhBaErrorMaSoNguoiDungFound = 504;
        public const int DanhBaErrorSoDienThoaiFound = 517;
        public const int DanhBaErrorSoDienThoaiInvalid = 505;
        public const int DanhBaErrorEmailInvalid = 506;
        public const int DanhBaErrorEmailFound = 507;
        public const int DanhBaErrorDanhBaChiTietNotFound = 508;
        public const int DanhBaErrorTruongDataNotFound = 509;
        public const int DanhBaErrorRequiredFieldNotFound = 510;
        public const int DanhBaErrorDataColumnMismatch = 511;
        public const int DanhBaErrorHoVaTenRequired = 512;
        public const int DanhBaErrorSoDienThoaiRequired = 513;
        public const int DanhBaErrorMaSoNguoiDungRequired = 514;
        public const int DanhBaErrorSoDienThoaiInvalidAtRow = 515;
        public const int DanhBaErrorMaSoNguoiDungFoundAtRow = 516;
        public const int DanhBaErrorDanhSachSoDienThoaiInvalid = 518;
        public const int DanhBaErrorDanhSachSoDienThoaiRequired = 519;

        public const int ToChucErrorNotFound = 601;
        public const int ToChucErrorLoaiToChucNotFound = 602;


        public const int ServiceAccountErrorNotFound = 701;
        public const int GoogleSheetUrlErrorInvalid = 702;

        public const int ImportHeaderErrorInvalid = 801;
        public const int ImportPhoneNumberErrorInvalid = 802;
        public const int ImportEmailErrorInvalid = 803;
        public const int ImportToChucErrorNotFound = 804;
        public const int ImportRequiredFieldErrorEmpty = 805;
        public const int ImportLoaiNguoiDungErrorInvalid = 806;

        public const int ImportExcelFileErrorEmpty = 807;
        public const int ImportExcelSheetNameErrorEmpty = 808;
        public const int ImportExcelSheetErrorNotFound = 809;
        public const int ImportExcelFileErrorInvalid = 810;
        public const int ImportDanhBaChienDichErrorMaSoNguoiDungDuplicate = 811;
        public const int ImportDanhBaChienDichErrorSoDienThoaiDuplicate = 812;

        public const int ErrorNoPermissionAccessGoogleSheet = 901;
        public const int ErrorServiceAccountNotFoundInAppSetting = 902;
        public const int ErrorServiceAccountNotFound = 903;





    }
}
