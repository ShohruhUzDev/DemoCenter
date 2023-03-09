using System;
using System.Data;
using System.Reflection.Metadata;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;

namespace DemoCenter.Services.Foundations.Subjects
{
    public partial class SubjectService
    {
        private void ValidateSubjectOnAdd(Subject subject)
        {
            ValidateSubjectNotNull(subject);

            Validate(
                (Rule: IsInvalid(subject.Id), Parameter: nameof(Subject.Id)),
                (Rule: IsInvalid(subject.SubjectName), Parameter: nameof(Subject.SubjectName)),
                (Rule: IsInvalid(subject.Price), Parameter: nameof(Subject.Price)),
                (Rule: IsInvalid(subject.CreatedDate), Parameter: nameof(Subject.CreatedDate)),
                (Rule: IsInvalid(subject.UpdatedDate), Parameter: nameof(Subject.UpdatedDate)),

                (Rule:IsInvalid(
                    firstDate:subject.CreatedDate,
                    secondDate:subject.UpdatedDate,
                    secondDateName:nameof(Subject.UpdatedDate)),
                    
                    Parameter:nameof(Subject.CreatedDate))) ;
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
        private static dynamic IsInvalid(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };
        private static dynamic IsInvalid(int price) => new
        {
            Condition = price == default,
            Message = "Value is required"
        };
        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Value is required"
        };
        private static void ValidateSubjectNotNull(Subject subject)
        {
            if (subject is null)
            {
                throw new NullSubjectException();
            }
        }

        public static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidSubjectExceptoion = new InvalidSubjectException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidSubjectExceptoion.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidSubjectExceptoion.ThrowIfContainsErrors();
        }

    }
}
