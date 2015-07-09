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

    /// <summary>
    /// The class enumerations.
    /// </summary>
    public static class Enumerations
    {
        /// <summary>
        /// Get ENUM Description.
        /// </summary>
        /// <param name="enumType">ENUM type.</param>
        /// <returns>String description of an ENUM.</returns>
        public static string GetDescription(Enum enumType)
        {
            FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return enumType.ToString();
            }
        }
    }
}
