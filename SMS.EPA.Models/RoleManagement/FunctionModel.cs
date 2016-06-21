namespace SMS.EPA.Model.RoleManagement
{
    public class FunctionModel
    {
        public int FunctionId { get; set; }

        public string FunctionKey { get; set; }

        public string FunctionName { get; set; }

        public int? ParentFunctionId { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsRead { get; set; }

        public bool IsHost { get; set; }
    }
}