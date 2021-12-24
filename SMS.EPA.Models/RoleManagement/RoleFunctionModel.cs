namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Models;

    public class RoleFunctionModel : Function
    {
        public bool IsRead { get; set; }
        public bool IsHost { get; set; }
        public new IList<RoleFunctionModel> Childs { get; set; }
    }
}