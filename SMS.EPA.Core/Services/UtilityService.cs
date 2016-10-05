namespace SMS.EPA.Core.Services
{
    using System;
    using System.IO;
    using System.Web;

    public class UtilityService : IUtilityService
    {
        public string GetAvatar(string code)
        {
            code += ".jpg";

            if (File.Exists(HttpContext.Current.Server.MapPath("/avatars/" + code)))
            {
                return "avatars/" + code;
            }

            return "avatars/avatar.jpg";
        }

        public double SplitNumber(double number, int numberOfDecimals)
        {
            double result = number;
            numberOfDecimals = numberOfDecimals <= 0 ? Constants.Constants.NumberOfDecimalDefault : numberOfDecimals;

            if ((number % 1) != 0)
            {
                int index = number.ToString().IndexOf('.');
                string naturalFraction = number.ToString().Substring(0, index),
                    decimalFraction = number.ToString().Substring(index + 1);

                result = double.Parse(naturalFraction + "." + decimalFraction.Substring(0, Math.Min(decimalFraction.Length, numberOfDecimals)));
            }

            return result;
        }
    }
}