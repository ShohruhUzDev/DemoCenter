using System;
using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;

namespace DemoCenter.Services.Foundations.Students
{
    public partial class StudentService
    {
        private void ValidateStudentOnAdd(Student student)
        {
            ValidationStudentNotNull(student);

            Validate(
                (Rule: IsInvalid(student.Id), Parameter: nameof(Student.Id)),
                (Rule: IsInvalid(student.FirstName), Parameter: nameof(Student.FirstName)),
                (Rule: IsInvalid(student.LastName), Parameter: nameof(Student.LastName)),
                (Rule: IsInvalid(student.Phone), Parameter: nameof(Student.Phone)),
                (Rule: IsInvalid(student.CreatedDate), Parameter: nameof(Student.CreatedDate)),
                (Rule: IsInvalid(student.UpdatedDate), Parameter: nameof(Student.UpdatedDate)),
                (Rule: IsNotRecent(student.CreatedDate), Parameter: nameof(Student.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: student.CreatedDate,
                    secondDate: student.UpdatedDate,
                    secondDateName: nameof(Student.UpdatedDate)),

                    Parameter: nameof(Student.CreatedDate)));

        }


        private void ValidateStudentOnModify(Student student)
        {
            ValidationStudentNotNull(student);

            Validate(
                (Rule: IsInvalid(student.Id), Parameter: nameof(Student.Id)),
                (Rule: IsInvalid(student.FirstName), Parameter: nameof(Student.FirstName)),
                (Rule: IsInvalid(student.LastName), Parameter: nameof(Student.LastName)),
                (Rule: IsInvalid(student.Phone), Parameter: nameof(Student.Phone)),
                (Rule: IsInvalid(student.CreatedDate), Parameter: nameof(Student.CreatedDate)),
                (Rule: IsInvalid(student.UpdatedDate), Parameter: nameof(Student.UpdatedDate)),
                (Rule: IsNotRecent(student.UpdatedDate), Parameter: nameof(student.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: student.UpdatedDate,
                    secondDate: student.CreatedDate,
                    secondDateName: nameof(Student.CreatedDate)),

                    Parameter: nameof(Student.UpdatedDate)));
        }

        private void ValidateAgainstStorageStudentOnModify(Student inputStudent, Student storageStudent)
        {
            ValidateStorageStudentExist(storageStudent, inputStudent.Id);

            Validate(
                (Rule: IsNotSame(
                    firstDate: inputStudent.CreatedDate,
                    secondDate: storageStudent.CreatedDate,
                    secondDateName: nameof(Student.CreatedDate)),
                    Parameter: nameof(Student.CreatedDate)),
                (Rule: IsSame(
                    firstDate: inputStudent.UpdatedDate,
                    secondDate: storageStudent.UpdatedDate,
                    secondDateName: nameof(Student.UpdatedDate)),
                    Parameter: nameof(Student.UpdatedDate)));
        }
        private static void ValidateStorageStudentExist(Student student, Guid studentId)
        {
            if (student is null)
                throw new NotFoundStudentException(studentId);
        }


        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };
        private static void ValidateStudentId(Guid studentId) =>
            Validate((Rule: IsInvalid(studentId), Parameter: nameof(Student.Id)));

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = dateTimeBroker.GetCurrentDateTime();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };
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


        private static void ValidationStudentNotNull(Student student)
        {
            if (student is null)
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
