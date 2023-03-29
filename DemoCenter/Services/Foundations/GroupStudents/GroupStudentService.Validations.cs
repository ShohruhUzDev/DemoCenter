using System;
using System.Net.Sockets;
using DemoCenter.Models.Groups.Exceptions;
using DemoCenter.Models.GroupStudents;

namespace DemoCenter.Services.Foundations.GroupStudents
{
    public partial class GroupStudentService
    {
        private void ValidateGroupStudentOnAdd(GroupStudent groupStudent)
        {
            ValidateGroupStudentNotNull(groupStudent);

            Validate(
                (Rule: IsInvalid(groupStudent.GroupId), Parameter: nameof(GroupStudent.GroupId)),
                (Rule: IsInvalid(groupStudent.Group), Parameter: nameof(GroupStudent.Group)),
                (Rule: IsInvalid(groupStudent.StudentId), Parameter: nameof(GroupStudent.StudentId)),
                (Rule: IsInvalid(groupStudent.Student), Parameter: nameof(GroupStudent.Student)),
                (Rule: IsInvalid(groupStudent.CreatedDate), Parameter: nameof(GroupStudent.CreatedDate)),
                (Rule: IsInvalid(groupStudent.UpdatedDate), Parameter: nameof(GroupStudent.UpdatedDate)));
              


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
