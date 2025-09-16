using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.shared.HttpRequest.Error
{
    public static class ErrorMessages
    {
        private static readonly Dictionary<int, string> _messages = new()
        {
            { ErrorCodes.System, "Lỗi hệ thống" },
            { ErrorCodes.InternalServerError, "Lỗi server" },
            { ErrorCodes.BadRequest, "Request không hợp lệ" },
            { ErrorCodes.NotFound, "Không tìm thấy trong hệ thống" },
            { ErrorCodes.Unauthorized, "Không được phân quyền" },
            { ErrorCodes.AuthErrorUserNotFound, "User không tồn tại" },
            { ErrorCodes.AuthErrorRoleNotFound, "Role không tồn tại" },
            { ErrorCodes.AuthInvalidPassword, "Mật khẩu không đúng" },
            { ErrorCodes.AuthErrorCreateUser, "Lỗi tạo user" },
            { ErrorCodes.AuthErrorCreateRole, "Lỗi tạo role" },
            { ErrorCodes.Found, "Đã tồn tại trong hệ thống" },
            { ErrorCodes.CuocHopErrorNotFound, "Cuộc họp không tồn tại" },
            { ErrorCodes.CuocHopErrorNameFound, "Tên cuộc họp đã tồn tại" },
            { ErrorCodes.CuocHopErrorTime, "Thời gian cuộc họp không hợp lệ" },
            { ErrorCodes.CuocHopErrorTimeDotDiemDanh, "Thời gian điểm danh cuộc họp không hợp lệ" },
            { ErrorCodes.CuocHopErrorInvalidJoinWebUrl, "Link tham gia cuộc họp không hợp lệ" },
            { ErrorCodes.CuocHopErrorMeetingNotFound, "Meeting không tồn tại" },
            { ErrorCodes.CuocHopErrorDotDiemDanhNotFound, "Đợt điểm danh không tồn tại" },
            { ErrorCodes.AuthErrorUserEmailHuceNotFound, "Không tìm thấy thông tin email HUCE của người dùng" },
            { ErrorCodes.CuocHopErrorDaDiemDanh, "Bạn đã điểm danh cho đợt này rồi"},
            { ErrorCodes.ChienDichErrorNotFound, "Chiến dịch không tồn tại"},
            { ErrorCodes.DanhBaErrorNotFound, "Danh bạ không tồn tại"},
            { ErrorCodes.ToChucErrorNotFound, "Tổ chức không tồn tại" },
            { ErrorCodes.ToChucErrorLoaiToChucNotFound,"Loại tổ chức không tồn tại" },
            { ErrorCodes.ServiceAccountErrorNotFound,"Không tìm thấy đường dẫn file service-account" },
            { ErrorCodes.ImportHeaderErrorInvalid, "Header không đúng định dạng tại dòng {0}" },
            { ErrorCodes.ImportPhoneNumberErrorInvalid, "Số điện thoại không đúng định dạng tại dòng {0}" },
            { ErrorCodes.ImportEmailHuceErrorInvalid, "Email Huce không đúng định dạng tại dòng {0}" },
            { ErrorCodes.ImportToChucErrorNotFound, "Tổ chức không tồn tại tại dòng {0}" },
            { ErrorCodes.ImportRequiredFieldErrorEmpty, "Trường bắt buộc không được để trống tại dòng {0}" },
            { ErrorCodes.ImportLoaiNguoiDungErrorInvalid, "Loại người dùng không hợp lệ tại dòng {0}" },
        };

        public static string GetMessage(int code)
        {
            return _messages.TryGetValue(code, out var message) ? message : "Unknown error.";
        }
    }

}
