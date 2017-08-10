using System;

namespace ApiTracker
{
    /// <summary>
    /// ApiTrackerEntity
    /// </summary>
    internal class ApiTrackerEntity
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// Action开始执行的时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Action执行完毕的时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Request的参数
        /// </summary>
        public string Params { get; set; }

        /// <summary>
        /// Request Headers
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        /// Request Method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Request IP
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// Action耗时 （EndTime - StartTime）
        /// </summary>
        public double UsedSeconds { get; set; }

        /// <summary>
        /// 调用者ID
        /// </summary>
        public long CallerID { get; set; }

        /// <summary>
        /// 调用者Name
        /// </summary>
        public string CallerName { get; set; }

        /// <summary>
        /// Action报错信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// for oauth client_id
        /// </summary>
        public string client_id { get; set; }
    }
}
