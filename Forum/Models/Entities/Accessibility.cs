using System.ComponentModel;

namespace Forum.Models.Entities
{
    public enum Accessibility
    {
        [Description("Full access")]
        FullAccess,
        [Description("Only for users")]
        OnlyForUsers,
        [Description("Readonly for users")]
        ReadonlyForUsers
    }
}
