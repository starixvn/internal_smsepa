namespace SMS.EPA.Model.RoleManagement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;

    public class RoleFunctionModel : FunctionModel
    {
        public RoleFunctionModel()
        {
            ChildFunctions = new List<RoleFunctionModel>();
        }

        public IEnumerable<RoleFunctionModel> ChildFunctions { get; set; }
    }
}