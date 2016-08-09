namespace SMS.EPA.Core.Model
{
    public class MenuItemModel
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public string MenuLink { get; set; }

        public int? ParentId { get; set; }

        public int FunctionId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}