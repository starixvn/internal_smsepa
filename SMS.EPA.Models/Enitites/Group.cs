namespace SMS.EPA.Model.Enitites
{
    using System;

    [Serializable]
    public class Group
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupCode { get; set; }
    }
}