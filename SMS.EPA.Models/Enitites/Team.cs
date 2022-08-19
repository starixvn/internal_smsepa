namespace SMS.EPA.Model.Enitites
{
    using System;

    [Serializable]
    public class Team
    {
        public int TeamId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? SectionId { get; set; }
    }
}