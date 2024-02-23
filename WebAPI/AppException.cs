using System;
using System.Globalization;

namespace WebAPI
{
    public class AppException : Exception
    {
        public AppException() : base() {}
        public AppException(string message): base(message) {}

        public AppException(string message, params object[] arg)
            : base(String.Format(CultureInfo.CurrentCulture, message, arg))
        {
            throw new NotImplementedException();
        }
    }
}