using System;
using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;

namespace DemoCenter.Services.Foundations.Students
{
    public partial class StudentService
    {
        private static void ValidateStudentOnAdd(Student student)
        {
            ValidationStudentNotNull(student);

            Validate(
                (Rule: IsInvalid(student.Id), Parameter: nameof(Student.Id)),
                (Rule: IsInvalid(student.FirstName), Parameter: nameof(Student.FirstName)),
                (Rule: IsInvalid(student.LastName), Parameter: nameof(Student.LastName)),
                (Rule: IsInvalid(student.Phone), Parameter: nameof(Student.Phone)),
                (Rule: IsInvalid(student.CreatedDate), Parameter: nameof(Student.CreatedDate)),
                (Rule: IsInvalid(student.UpdatedDate), Parameter: nameof(Student.UpdatedDate)),

                (Rule: IsInvalid(
                    firstDate: student.CreatedDate,
                    secondDate: student.UpdatedDate,
                    secondDateName: nameof(Student.UpdatedDate)),

                    Parameter: nameof(Student.CreatedDate)));

        }

        private static dynamic IsInvalid(Guid id) => new
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
            Message = "Value is required"
        };

        private static dynamic IsInvalid(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private static void ValidationStudentNotNull(Student student)
        {
            if (student == null)
            {
                throw new NullStudentException();
            }
        }
        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidStudentException = new InvalidStudentException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidStudentException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidStudentException.ThrowIfContainsErrors();
        }
    }
}
