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

        public const int DanhBaErrorNotFound = 501;


        public const int ToChucErrorNotFound = 601;
        public const int ToChucErrorLoaiToChucNotFound = 602;
        


    }
}
