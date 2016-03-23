// ----------------------------------------------------------------------
// <copyright file="Enumerations.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 2:35:35 PM">Created.</change>
// </history>

namespace SMS.EPA.Core.Utilities
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public static class Enumerations
    {
        public static string GetDescription(Enum enumType)
        {
            string desciption = string.Empty;

            if (enumType != null)
            {
                FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    desciption = attributes[0].Description;
                }
                else
                {
                    desciption = enumType.ToString();
                }
            }

            return desciption;
        }
    }
}
