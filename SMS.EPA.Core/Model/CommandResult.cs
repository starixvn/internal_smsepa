namespace SMS.EPA.Core.Model
{
    using Resources;

    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = Lang.InvalidInputData;
        }

        public int ObjectId { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful => Code == 0;
    }
}