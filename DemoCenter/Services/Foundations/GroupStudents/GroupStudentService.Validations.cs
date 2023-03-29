using System;
using System.Net.Sockets;
using DemoCenter.Models.Groups.Exceptions;
using DemoCenter.Models.GroupStudents;

namespace DemoCenter.Services.Foundations.GroupStudents
{
    public partial class GroupStudentService
    {
        private void ValidateGroupStudentOnAdd(GroupStudent GroupStudent)
        {
            ValidateGroupStudentNotNull(GroupStudent);
           
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
