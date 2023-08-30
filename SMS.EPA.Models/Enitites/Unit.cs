namespace SMS.EPA.Model.Enitites
{
    using System;

    [Serializable]
    public class Unit
    {
        public int UnitId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}