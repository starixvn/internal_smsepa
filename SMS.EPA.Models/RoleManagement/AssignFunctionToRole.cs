namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using GO.UserControl.Client.Core.Models;

    public class AssignFunctionToRole
    {
        public short RoleId { get; set; }
        public IList<FunctionPermission> Permissions { get; set; }
    }
}