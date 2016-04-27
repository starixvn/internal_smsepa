using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ClientHRM.Model
{
    public class CommandResult
    {
        public CommandResult()
        {
            Code = 0;
            Message = string.Empty;
        }

        public int Id { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful
        {
            get
            {
                return Code == 0;
            }
        }
    }
}
