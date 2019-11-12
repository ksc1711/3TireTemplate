using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCMS.Web
{
    public enum FileUploadStatus
    {
        OK = 0,

        ZERO_BYTE = 1,

        NOT_SUPPORT_EXTENSIONTYPE = 2,

        SYSTEM_EXCEPTION = 4,
    }
}