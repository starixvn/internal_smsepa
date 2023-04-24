namespace SMS.EPA.ServicesAPI.Models.Todolist
{
    using System;

    [Serializable]
    public class TodolistAction
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public bool IsRedirect { get; set; }
    }
}