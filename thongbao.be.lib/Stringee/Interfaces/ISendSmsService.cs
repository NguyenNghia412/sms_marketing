using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.lib.Stringee.Interfaces
{
    public interface ISendSmsService
    {
        public Task<object> SendSmsAsync(List<object> smsMessages);
    }
}
