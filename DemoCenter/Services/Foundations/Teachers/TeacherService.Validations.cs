using System;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;

namespace DemoCenter.Services.Foundations.Teachers
{
    public partial class TeacherService
    {
        public void ValidationTeacherOnAdd(Teacher teacher)
        {
            ValidationTeacherNotNull(teacher);

            Validate(
                (Rule: IsInvalid(teacher.Id), Parameter: nameof(Teacher.Id)),
                (Rule: IsInvalid(teacher.FirstName), Parameter: nameof(Teacher.FirstName)),
                (Rule: IsInvalid(teacher.LastName), Parameter: nameof(Teacher.LastName)),
                (Rule: IsInvalid(teacher.CreatedDate), Parameter: nameof(Teacher.CreatedDate)),
                (Rule: IsInvalid(teacher.UpdatedDate), Parameter: nameof(Teacher.UpdatedDate)));

        }
        public static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Value si required"
        };
        private static void ValidationTeacherNotNull(Teacher teacher)
        {
            if (teacher is null)
            {
                throw new NullTeacherException();

            }
        }
        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidTeacherExceptioin = new InvalidTeacherException();

            foreach((dynamic rule, string parameter) in validations)

            {
                if(rule.Condition)
                {
                    invalidTeacherExceptioin.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidTeacherExceptioin.ThrowIfContainsErrors();
        }
    }
}
