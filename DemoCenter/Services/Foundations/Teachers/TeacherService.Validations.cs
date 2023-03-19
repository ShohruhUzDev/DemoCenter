using System;
using System.Data;
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
                (Rule: IsInvalid(teacher.UpdatedDate), Parameter: nameof(Teacher.UpdatedDate)),

                (Rule: IsInvalid(
                    firstDate: teacher.CreatedDate,
                    secondDate: teacher.UpdatedDate,
                    secondDateName: nameof(Teacher.UpdatedDate)),
                Parameter: nameof(Teacher.CreatedDate)));

        }

        public void ValidationTeacherOnModify(Teacher teacher)
        {
            ValidationTeacherNotNull(teacher);

            Validate(
                (Rule: IsInvalid(teacher.Id), Parameter: nameof(Teacher.Id)),
                (Rule: IsInvalid(teacher.FirstName), Parameter: nameof(Teacher.FirstName)),
                (Rule: IsInvalid(teacher.LastName), Parameter: nameof(Teacher.LastName)),
                (Rule: IsInvalid(teacher.CreatedDate), Parameter: nameof(Teacher.CreatedDate)),
                (Rule: IsInvalid(teacher.UpdatedDate), Parameter: nameof(Teacher.UpdatedDate)),
                (Rule: IsNotRecent(teacher.UpdatedDate), Parameter: nameof(Teacher.UpdatedDate)),

            (Rule: IsSame(
                firstDate: teacher.UpdatedDate,
                secondDate: teacher.CreatedDate,
                secondDateName: nameof(Teacher.CreatedDate)),
            Parameter: nameof(Teacher.UpdatedDate)));
        }

        private static void ValidateAgainstTeacherOnModify(Teacher inputTeacher, Teacher storageTeacher)
        {
            ValidateStoreageTeacherExist(storageTeacher, inputTeacher.Id);
        }

        private static void ValidateStoreageTeacherExist(Teacher teacher, Guid teacherId)
        {
            if (teacher is null)
                throw new NotFoundTeacherException(teacherId);
        }

        private dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = this.dateTimeBroker.GetCurrenDateTime();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }
        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message="Date is not recent"
        };

        private static void ValidateTeacherId(Guid teacherId) =>
            Validate((Rule: IsInvalid(teacherId), Parameter: nameof(Teacher.Id)));

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

        private static dynamic IsInvalid(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
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

            foreach ((dynamic rule, string parameter) in validations)

            {
                if (rule.Condition)
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
