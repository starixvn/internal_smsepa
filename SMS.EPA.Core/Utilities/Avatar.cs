// -----------------------------------------------------------------------
// <copyright file="Avatar.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.12">Created</change>
// </history>

namespace SMS.EPA.Core.Utilities
{
    using System.IO;
    using System.Web;

    public static class Avatar
    {
        public static string GetAvatar(string code)
        {
            code += ".jpg";

            if (File.Exists(HttpContext.Current.Server.MapPath("/avatars/" + code)))
            {
                return "/avatars/" + code;
            }

            return "/avatars/avatar.jpg";
        }
    }
}