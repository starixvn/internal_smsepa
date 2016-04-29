namespace SMS.EPA.ClientHRM.Model
{
    public class CommandResult
    {
        public CommandResult()
        {
            Code = 0;
            Message = string.Empty;
        }

        public int Code { get; set; }

        public string Message { get; set; }
    }
}
