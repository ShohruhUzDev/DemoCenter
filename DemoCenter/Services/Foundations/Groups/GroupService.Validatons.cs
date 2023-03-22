using System;
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

                (Rule: IsNotSame(
                    firstDate: group.CreatedDate,
                    secondDate: group.UpdatedDate,
                    secondDateName: nameof(Group.UpdatedDate)),

                    Parameter: nameof(Group.CreatedDate)),
                (Rule: IsNotRecent(group.CreatedDate), Parameter: nameof(Group.CreatedDate)));
        }

        private void ValidateGroupOnModify(Group group)
        {
            ValidationGroupNotNull(group);

            Validate(
              (Rule: IsInvalid(group.Id), Parameter: nameof(Group.Id)),
              (Rule: IsInvalid(group.GroupName), Parameter: nameof(Group.GroupName)),
              (Rule: IsInvalid(group.UpdatedDate), Parameter: nameof(Group.UpdatedDate)),
              (Rule: IsInvalid(group.CreatedDate), Parameter: nameof(Group.CreatedDate)),
              (Rule: IsNotRecent(group.UpdatedDate), Parameter: nameof(Group.UpdatedDate)),

            (Rule: IsSame(
                  firstDate: group.UpdatedDate,
                  secondDate: group.CreatedDate,
                  secondDateName: nameof(Group.CreatedDate)),

                  Parameter: nameof(Group.UpdatedDate)));

        }

        private void ValidateAgainstStorageGroupOnModify(Group inputGroup, Group storageGroup)
        {
            ValidateStorageGroupExist(storageGroup, inputGroup.Id);

            Validate(
                (Rule: IsNotSame(
                    firstDate: inputGroup.CreatedDate,
                    secondDate: storageGroup.CreatedDate,
                    secondDateName: nameof(Group.CreatedDate)),
                Parameter: nameof(Group.CreatedDate)),
            (Rule: IsSame(
                    firstDate: inputGroup.UpdatedDate,
                    secondDate: storageGroup.UpdatedDate,
                    secondDateName: nameof(Group.UpdatedDate)),
                Parameter: nameof(Group.UpdatedDate)));

        }

        private static dynamic IsSame(
           DateTimeOffset firstDate,
           DateTimeOffset secondDate,
           string secondDateName) => new
           {
               Condition = firstDate == secondDate,
               Message = $"Date is the same as {secondDateName}"
           };


        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };


        private static void ValidateGroupId(Guid groupId) =>
            Validate((Rule: IsInvalid(groupId), Parameter: (nameof(Group.Id))));

        private static void ValidateStorageGroupExist(Group maybeGroup, Guid groupId)
        {
            if (maybeGroup is null)
            {
                throw new NotFoundGroupException(groupId);
            }
        }

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = this.dateTimeBroker.GetCurrenDateTime();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

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
