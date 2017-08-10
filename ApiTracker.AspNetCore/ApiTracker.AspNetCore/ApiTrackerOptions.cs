using System;

namespace ApiTracker
{
    /// <summary>
    /// API日志记录配置项
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiTrackerOptions : Attribute
    {
        public bool EnableHeaders { get; set; }

        public bool EnableClientIp { get; set; }

        public bool EnableParams { get; set; }

        public ApiTrackerOptions() : this(true, true, true)
        {

        }

        public ApiTrackerOptions(bool EnableHeaders, bool EnableClientIp, bool EnableParams)
        {
            this.EnableHeaders = EnableHeaders;
            this.EnableClientIp = EnableClientIp;
            this.EnableParams = EnableParams;
        }
    }
}
