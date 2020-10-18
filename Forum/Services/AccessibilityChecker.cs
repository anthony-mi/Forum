using Forum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    public class AccessibilityChecker
    {
        public static bool HasAccess(IList<string> userRoles, Accessibility accessibility)
        {
            return  accessibility == Accessibility.FullAccess ||
                    (accessibility == Accessibility.OnlyForUsers && userRoles.Contains("User")) ||
                    (accessibility == Accessibility.ReadonlyForUsers && userRoles.Contains("User"))
                    || userRoles.Contains("Moderator")
                    || userRoles.Contains("Admin");
        }
    }
}
