using System;
using System.Data;
using System.Reflection.Metadata;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;

namespace DemoCenter.Services.Foundations.Groups
{
    public partial class GroupService
    {
        private void ValidateGroupOnAdd(Group group)
        {
            ValidationGroupNotNull(group);

            Validate(
                (Rule: IsInvalid(group.Id), Parameter: nameof(Group.Id)),
                (Rule: IsInvalid(group.GroupName), Parameter: nameof(Group.GroupName)),
                (Rule: IsInvalid(group.UpdatedDate), Parameter: nameof(Group.UpdatedDate)),
                (Rule: IsInvalid(group.CreatedDate), Parameter: nameof(Group.CreatedDate)),
           
                (Rule:IsInvalid(
                    firstDate:group.CreatedDate,
                    secondDate:group.UpdatedDate,
                    secondDateName:nameof(Group.UpdatedDate)),
                    
                    Parameter:nameof(Group.CreatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required"
        };
        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Value is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };
        private static dynamic IsInvalid(
           DateTimeOffset firstDate,
           DateTimeOffset secondDate,
           string secondDateName) => new
           {
               Condition = firstDate != secondDate,
               Message = $"Date is not same as {secondDateName}"
           };
        private static void ValidationGroupNotNull(Group group)
        {
            if (group is null)
            {
                throw new NullGroupException();
            }
        }
        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidGroupException = new InvalidGroupException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidGroupException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidGroupException.ThrowIfContainsErrors();
        }
    }
}
