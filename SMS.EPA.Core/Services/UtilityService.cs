namespace SMS.EPA.Core.Services
{
    using System;
    using System.IO;
    using System.Web;

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

            var ofDecimals = numberOfDecimals <= 0
                ? Constants.Constants.NumberOfDecimalDefault
                : numberOfDecimals;

            if ((number % 1) != 0)
            {
                result = Math.Truncate(number * Math.Pow(10, ofDecimals)) / Math.Pow(10, ofDecimals);
            }

            return result;
        }
    }
}