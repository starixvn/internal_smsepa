namespace SMS.EPA.Model.Enitites
{
    using System;

    [Serializable]
    public class Section
    {
        public int SectionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? DepartmentId { get; set; }
    }
}