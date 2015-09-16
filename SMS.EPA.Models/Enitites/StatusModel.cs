// -----------------------------------------------------------------------
// <copyright file="StatusModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Model.Enitites
{
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities;

    public class StatusModel
    {
        public int StatusId { get; set; }

        public string Name
        {
            get
            {
                return Enumerations.GetDescription((Status)StatusId);
            }
        }
    }
}