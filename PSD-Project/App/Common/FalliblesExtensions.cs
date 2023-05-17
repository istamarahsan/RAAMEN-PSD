using System;
using Util.Try;

namespace PSD_Project.App.Common
{
    public static class FalliblesExtensions
    {
        public static Try<int, Exception> TryParseInt(this string str)
        {
            try
            {
                return Try.Of<int, Exception>(int.Parse(str));
            }
            catch (Exception e)
            {
                return Try.Err<int, Exception>(e);
            }
        }

        public static Try<double, Exception> TryParseDouble(this string str)
        {
            try
            {
                return Try.Of<double, Exception>(double.Parse(str));
            }
            catch (Exception e)
            {
                return Try.Err<double, Exception>(e);
            }
        }
    }
}