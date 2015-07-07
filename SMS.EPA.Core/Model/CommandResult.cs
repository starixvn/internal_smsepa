// ----------------------------------------------------------------------
// <copyright file="CommandResult.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="" date="7/6/2015 2:29:36 PM"></change>
// </history>

namespace SMS.EPA.Core.Model
{
    using SMS.EPA.Resources;

    /// <summary>
    /// The command result model.
    /// </summary>
    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = Lang.InvalidInputData;
        }

        public int Id { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful
        {
            get
            {
                return Code == 0;
            }
        }
    }
}
