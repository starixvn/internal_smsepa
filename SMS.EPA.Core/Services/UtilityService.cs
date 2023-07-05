namespace SMS.EPA.Core.Services
{
    using System;
    using System.IO;
    using System.Web;
    using SMS.EPA.Core.Constants;

    public class UtilityService : IUtilityService
    {
        public string GetAvatar(string code)
        {
            string fileName = $"{code}.jpg";

            fileName = File.Exists(HttpContext.Current.Server.MapPath("/avatars/" + fileName))
                ? "avatars/" + fileName
                : "avatars/avatar.jpg";

            return fileName;
        }

        public double SplitNumber(double number, int numberOfDecimals)
        {
            double result = number;

            var decimals = numberOfDecimals <= 0
                ? Constants.NumberOfDecimalDefault
                : numberOfDecimals;

            if (Math.Abs(number % 1) > Constants.Tolerance)
            {
                const byte RaisingNumber = 10;
                result = Math.Truncate(number * Math.Pow(RaisingNumber, decimals)) / Math.Pow(RaisingNumber, decimals);
            }

            return result;
        }
    }
}