namespace SMS.EPA.Model
{
    public class EPASummaryCommentModel
    {
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Section { get; set; }
        public string Team { get; set; }
        public string StaffComment { get; set; }
        public string EvaluatorNickName { get; set; }
        public string EvaluatorComment { get; set; }
        public byte ProcessId { get; set; }
    }
}