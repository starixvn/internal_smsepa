namespace SMS.EPA.Core.Services
{
    using Constants;
    using Enumerations;
    using Model;
    using System.Linq;
    using System.Web;

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

        public int GetPermission(string functionKey)
        {
            int policy = (int)Policy.None;

            if (HasPermissionOnFunction(functionKey, Policy.Host))
            {
                policy = (int)Policy.Host;
            }
            else if (HasPermissionOnFunction(functionKey, Policy.Read))
            {
                policy = (int)Policy.Read;
            }

            return policy;
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            FunctionModel currentFunction =
                CurrentUser?.Functions.FirstOrDefault(func => func.FunctionKey.Equals(functionKey));

            if (currentFunction != null)
            {
                switch (policy)
                {
                    case Policy.None:
                        return false;

                    case Policy.Read:
                        return currentFunction.IsRead || currentFunction.IsHost;

                    case Policy.Host:
                        return currentFunction.IsHost;
                }
            }

            return false;
        }
    }
}