namespace SMS.EPA.Core.Services
{
    using System.Linq;
    using System.Web;
    using Constants;
    using Enumerations;
    using Model;

    public class SessionService : ISessionService
    {
        public UserSession CurrentUser
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    return (UserSession)HttpContext.Current.Session[Constants.CurrentSessionKey];
                }

                return null;
            }

            set
            {
                HttpContext.Current.Session[Constants.CurrentSessionKey] = value;
            }
        }

        public int EmployeeId
        {
            get { return CurrentUser.EmployeeId; }

            set { CurrentUser.EmployeeId = value; }
        }

        public byte GetPermission(string functionKey)
        {
            byte policy = (byte)Policy.None;
            if (HasPermissionOnFunction(functionKey, Policy.Host))
            {
                policy = (byte)Policy.Host;
            }
            else
            {
                if (HasPermissionOnFunction(functionKey, Policy.Read))
                {
                    policy = (byte)Policy.Read;
                }
            }

            return policy;
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            FunctionModel currentFunction =
                CurrentUser?.Functions.FirstOrDefault(func => func.FunctionKey.Equals(functionKey));
            var result = false;

            if (currentFunction != null)
            {
                switch (policy)
                {
                    case Policy.None:
                        result = false;
                        break;

                    case Policy.Read:
                        result = currentFunction.IsRead || currentFunction.IsHost;
                        break;

                    case Policy.Host:
                        result = currentFunction.IsHost;
                        break;

                    default:
                        result = false;
                        break;
                }
            }

            return result;
        }
    }
}