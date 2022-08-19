namespace SMS.EPA.Model.Enitites
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MultiSelectItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public bool Enabled { get; set; } = true;
        public int? ParentId { get; set; }
        public bool IsLeaf { get; set; }
        public IList<MultiSelectItem> Items { get; set; } = new List<MultiSelectItem>();

    }
}