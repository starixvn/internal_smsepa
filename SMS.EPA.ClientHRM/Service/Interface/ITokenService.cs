using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.ClientHRM.Service
{
    public interface ITokenService
    {
        TokenAccess Request(ApplicationInfo info);
    }
}
