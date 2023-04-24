namespace SMS.EPA.ServicesAPI.Models.Todolist
{
    using System;

    public class TodolistEPA
    {
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public byte ProcessId { get; set; }
        public short TargetTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}