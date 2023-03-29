using System;
using DemoCenter.Models.Groups.Exceptions;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Models.GroupStudents.Exceptions;

namespace DemoCenter.Services.Foundations.GroupStudents
{
    public partial class GroupStudentService
    {
        private void ValidateGroupStudentOnAdd(GroupStudent groupStudent)
        {
            ValidateGroupStudentNotNull(groupStudent);

            Validate(
                (Rule: IsInvalid(groupStudent.GroupId), Parameter: nameof(GroupStudent.GroupId)),
                (Rule: IsInvalid(groupStudent.StudentId), Parameter: nameof(GroupStudent.StudentId)),
                (Rule: IsInvalid(groupStudent.CreatedDate), Parameter: nameof(GroupStudent.CreatedDate)),
                (Rule: IsInvalid(groupStudent.UpdatedDate), Parameter: nameof(GroupStudent.UpdatedDate)),
                (Rule: IsNotRecent(groupStudent.CreatedDate), Parameter: nameof(GroupStudent.CreatedDate)),

            (Rule: IsNotSame(
                    firstDate: groupStudent.UpdatedDate,
                    secondDate: groupStudent.CreatedDate,
                    secondDateName: nameof(GroupStudent.CreatedDate)),
                Parameter: nameof(GroupStudent.UpdatedDate)));

        }

        private void ValidateGroupStudentOnModify(GroupStudent groupStudent)
        {
            ValidateGroupStudentNotNull(groupStudent);

            Validate(
                (Rule: IsInvalid(groupStudent.StudentId), Parameter: nameof(GroupStudent.StudentId)),
                (Rule: IsInvalid(groupStudent.GroupId), Parameter: nameof(GroupStudent.GroupId)),
                (Rule: IsInvalid(groupStudent.CreatedDate), Parameter: nameof(GroupStudent.CreatedDate)),
                (Rule: IsInvalid(groupStudent.UpdatedDate), Parameter: nameof(GroupStudent.UpdatedDate)),
                (Rule: IsNotRecent(groupStudent.UpdatedDate), Parameter: nameof(GroupStudent.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: groupStudent.UpdatedDate,
                    secondDate: groupStudent.CreatedDate,
                    secondDateName: nameof(GroupStudent.CreatedDate)),

                 Parameter: nameof(GroupStudent.UpdatedDate)));
        }

        private void ValidateAgainstStorageGroupStudentOnModify(GroupStudent inputGroupStudent, GroupStudent storageGroupStudent)
        {
            ValidateStorageGroupStudent(storageGroupStudent, inputGroupStudent.GroupId, inputGroupStudent.StudentId);

            Validate(
                (Rule: IsNotSame(
                    firstDate: inputGroupStudent.CreatedDate,
                    secondDate: storageGroupStudent.CreatedDate,
                    secondDateName: nameof(GroupStudent.CreatedDate)),
                Parameter: nameof(GroupStudent.CreatedDate)),

                (Rule: IsSame(
                        firstDate: inputGroupStudent.UpdatedDate,
                        secondDate: storageGroupStudent.UpdatedDate,
                        secondDateName: nameof(GroupStudent.UpdatedDate)),
                    Parameter: nameof(GroupStudent.UpdatedDate)));
        }

        private static void ValidateGroupStudentNotNull(GroupStudent GroupStudent)
        {
            if (GroupStudent is null)
            {
                throw new NullGroupStudentException();
            }
        }

        private static void ValidateStorageGroupStudent(GroupStudent maybeGroupStudent, Guid groupId, Guid studentId)
        {
            if (maybeGroupStudent is null)
            {
                throw new NotFoundGroupStudentException(groupId, studentId);
            }
        }

        private void ValidateGroupStudentIds(Guid groupId, Guid studentId)
        {
            Validate(
                 (Rule: IsInvalid(groupId), Parameter: nameof(GroupStudent.GroupId)),
                 (Rule: IsInvalid(studentId), Parameter: nameof(GroupStudent.StudentId)));

        }

        private static dynamic IsSame(
          DateTimeOffset firstDate,
          DateTimeOffset secondDate,
          string secondDateName) => new
          {
              Condition = firstDate == secondDate,
              Message = $"Date is the same as {secondDateName}"
          };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = this.dateTimeBroker.GetCurrentDateTime();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private static dynamic IsNotSame(
          DateTimeOffset firstDate,
          DateTimeOffset secondDate,
          string secondDateName) => new
          {
              Condition = firstDate != secondDate,
              Message = $"Date is not the same as {secondDateName}"
          };
        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Value is required"
        };

        private static dynamic IsInvalid(object @object) => new
        {
            Condition = @object is null,
            Message = "Object is required"
        };
        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidTeamException = new InvalidGroupStudentException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidTeamException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidTeamException.ThrowIfContainsErrors();
        }

    }
}
