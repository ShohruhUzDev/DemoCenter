using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions.Common;

namespace DemoCenter.Services.Foundations.Groups
{
    public partial class GroupService
    {
        private static void ValidationGroupNotNull(Group group)
        {
            if (group is null)
            {
                throw new NullGroupException();
            }
        }
    }
}
