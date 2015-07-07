// ----------------------------------------------------------------------
// <copyright file="Group.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 10:48:44 AM">Created.</change>
// </history>

namespace SMS.EPA.Model.Enitites
{
    /// <summary>
    /// The group entity.
    /// </summary>
    public class Group
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupCode { get; set; }

        public bool IsDeleted { get; set; }
    }
}
