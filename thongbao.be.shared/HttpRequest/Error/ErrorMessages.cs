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
            { ErrorCodes.ImportEmailErrorInvalid, "Email  không đúng định dạng tại dòng {0}" },
            { ErrorCodes.ImportToChucErrorNotFound, "Tổ chức không tồn tại tại dòng {0}" },
            { ErrorCodes.ImportRequiredFieldErrorEmpty, "Trường bắt buộc không được để trống tại dòng {0}" },
            { ErrorCodes.ImportLoaiNguoiDungErrorInvalid, "Loại người dùng không hợp lệ tại dòng {0}" },
            { ErrorCodes.DanhBaErrorRequired ,"Các trường thông tin không được để trống" },
            { ErrorCodes.DanhBaErrorMaSoNguoiDungFound,"Mã số người dùng đã tồn tại" },
            { ErrorCodes.DanhBaErrorSoDienThoaiInvalid,"Định dạng số điện thoại không đúng " },
            { ErrorCodes.DanhBaErrorEmailInvalid,"Định dạng email không đúng " },
            { ErrorCodes.DanhBaErrorEmailFound,"Email đã tồn tại" },
            { ErrorCodes.ImportExcelFileErrorEmpty, "File Excel không được để trống" },
            { ErrorCodes.ImportExcelSheetNameErrorEmpty, "Tên sheet không được để trống" },
            { ErrorCodes.ImportExcelSheetErrorNotFound, "Không tìm thấy sheet '{0}' trong file Excel" },
            { ErrorCodes.ImportExcelFileErrorInvalid, "File Excel không hợp lệ" },
            { ErrorCodes.ImportDanhBaChienDichErrorMaSoNguoiDungDuplicate, "Mã số người dùng '{0}' bị trùng lặp tại dòng {1}" },
            { ErrorCodes.ImportDanhBaChienDichErrorSoDienThoaiDuplicate,"Số điện thoại '{0}' bị trùng lặp tại dòng {1}" },
            { ErrorCodes.MauNoiDungErrorNotFound , "Mẫu nội dung không tồn tại" },
            { ErrorCodes.DanhBaErrorDanhBaChiTietNotFound,"Danh bạ chi tiết người dùng không tồn tại" },
            { ErrorCodes.ChienDichErrorBrandNameNotFound,"BrandName không tồn tại" },
            { ErrorCodes.DanhBaErrorTruongDataNotFound, "Không tìm thấy trường dữ liệu trong danh bạ" },
            { ErrorCodes.DanhBaErrorRequiredFieldNotFound, "Không tìm thấy các trường bắt buộc: Họ và tên, Số điện thoại" },
            { ErrorCodes.DanhBaErrorDataColumnMismatch, "Số lượng cột dữ liệu không khớp với số trường tại dòng {0}" },
            { ErrorCodes.DanhBaErrorHoVaTenRequired, "Họ và tên không được để trống tại dòng {0}" },
            { ErrorCodes.DanhBaErrorSoDienThoaiRequired, "Số điện thoại không được để trống tại dòng {0}" },
            { ErrorCodes.DanhBaErrorMaSoNguoiDungRequired, "Mã số người dùng không được để trống tại dòng {0}" },
            { ErrorCodes.DanhBaErrorSoDienThoaiInvalidAtRow, "Số điện thoại không đúng định dạng tại dòng {0}" },
            { ErrorCodes.DanhBaErrorMaSoNguoiDungFoundAtRow, "Mã số người dùng '{0}' đã tồn tại tại dòng {1}" },
            { ErrorCodes.ChienDichErrorTrangThaiTrue,"Chiến dịch này đã được gửi. Yêu cầu nhân bản chiến dịch để có thể thực hiện tiếp lệnh gửi" },
            { ErrorCodes.DanhBaErrorSoDienThoaiFound,"Số điện thoại '{0}' đã tồn tại tại dòng {1}" },
            { ErrorCodes.ChienDichErrorTrangThaiTrueCannotDelete,"Chiến dịch đã được gửi, không thể xóa"},
            { ErrorCodes.GoogleSheetUrlErrorInvalid, "URL Google Sheet không hợp lệ hoặc không thể truy cập được" },
            { ErrorCodes.ErrorNoPermissionAccessGoogleSheet,"Không có quyền truy cập vào Google Sheet" },
            { ErrorCodes.ErrorServiceAccountNotFoundInAppSetting,"Không tìm thấy config đường dẫn của service-account trong appsetting.json" },
            { ErrorCodes.ErrorServiceAccountNotFound,"Không tim thấy file service-account.json" },
            { ErrorCodes.DanhBaErrorDanhSachSoDienThoaiInvalid, "Số điện thoại '{0}' không hợp lệ (phải đủ 10 số)" },
            { ErrorCodes.DanhBaErrorDanhSachSoDienThoaiRequired, "Vui lòng cung cấp IdDanhBa hoặc DanhSachSoDienThoai" },
        };

        public static string GetMessage(int code)
        {
            return _messages.TryGetValue(code, out var message) ? message : "Unknown error.";
        }
    }

}
