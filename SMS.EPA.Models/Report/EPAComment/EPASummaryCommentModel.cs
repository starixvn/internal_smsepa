namespace SMS.EPA.Model
{
    public class EPASummaryCommentModel
    {
        public string Code { get; set; }
        public string NickName { get; set; }
        public string Section { get; set; }
        public string StaffComment { get; set; }
        public string EvaluatorNickName { get; set; }
        public string EvaluatorComment { get; set; }
        public byte ProcessId { get; set; }
    }
}