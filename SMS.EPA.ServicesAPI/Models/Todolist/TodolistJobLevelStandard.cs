namespace SMS.EPA.ServicesAPI.Models.Todolist
{
    using System;

    [Serializable]
    public class TodolistJobLevelStandard
    {
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public byte ProcessId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}