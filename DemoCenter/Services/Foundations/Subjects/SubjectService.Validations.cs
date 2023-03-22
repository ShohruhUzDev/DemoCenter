using System;
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
                (Rule: IsNotRecent(subject.CreatedDate), Parameter: nameof(Subject.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: subject.CreatedDate,
                    secondDate: subject.UpdatedDate,
                    secondDateName: nameof(Subject.UpdatedDate)),

                    Parameter: nameof(Subject.CreatedDate)));
        }

        private void ValidateSubjectOnModify(Subject subject)
        {
            ValidateSubjectNotNull(subject);

            Validate(
              (Rule: IsInvalid(subject.Id), Parameter: nameof(Subject.Id)),
              (Rule: IsInvalid(subject.SubjectName), Parameter: nameof(Subject.SubjectName)),
              (Rule: IsInvalid(subject.Price), Parameter: nameof(Subject.Price)),
              (Rule: IsInvalid(subject.CreatedDate), Parameter: nameof(Subject.CreatedDate)),
              (Rule: IsInvalid(subject.UpdatedDate), Parameter: nameof(Subject.UpdatedDate)),
              (Rule: IsNotRecent(subject.UpdatedDate), Parameter: nameof(Subject.UpdatedDate)),

              (Rule: IsSame(
                  firstDate: subject.UpdatedDate,
                  secondDate: subject.CreatedDate,
                  secondDateName: nameof(Subject.CreatedDate)),

                  Parameter: nameof(Subject.UpdatedDate)));
        }
        private void ValidateAginstStorageSubjectOnModify(Subject inputSubject, Subject storageSubject)
        {
            ValidatStorageSubjectExist(storageSubject, inputSubject.Id);

            Validate(
               (Rule: IsNotSame(
                   firstDate: inputSubject.CreatedDate,
                   secondDate: storageSubject.CreatedDate,
                   secondDateName: nameof(Subject.CreatedDate)),
                   Parameter: nameof(Subject.CreatedDate)),

            (Rule: IsSame(
                   firstDate: inputSubject.UpdatedDate,
                   secondDate: storageSubject.UpdatedDate,
                   secondDateName: nameof(Subject.UpdatedDate)),
                   Parameter: nameof(Subject.UpdatedDate)));

        }

        private static void ValidatStorageSubjectExist(Subject subject, Guid subjectId)
        {
            if (subject is null)
                throw new NotFoundSubjectException(subjectId);
        }

        private static void ValidateSubjectId(Guid subjectId) =>
            Validate((Rule: IsInvalid(subjectId), Parameter: nameof(Subject.Id)));

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };
   
        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = this.dateTimeBroker.GetCurrenDateTime();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
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
