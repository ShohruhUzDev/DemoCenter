using System;
using DemoCenter.Models.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;

namespace DemoCenter.Services.Foundations.Users
{
    public partial class UserService
    {

        private void ValidateUserOnAdd(User user)
        {
            ValidateUserNotNull(user);

            Validate(
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.FirstName), Parameter: nameof(User.FirstName)),
                (Rule: IsInvalid(user.LastName), Parameter: nameof(User.LastName)),
                (Rule: IsInvalid(user.PhoneNumber), Parameter: nameof(User.PhoneNumber)),
                (Rule: IsInvalid(user.Email), Parameter: nameof(User.Email)),
                (Rule: IsInvalid(user.BirthDate), Parameter: nameof(User.BirthDate)),
                (Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
                (Rule: IsInvalid(user.Password), Parameter: nameof(User.Password)),
                (Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)),
                (Rule: IsNotRecent(user.CreatedDate), Parameter: nameof(User.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: user.CreatedDate,
                    secondDate: user.UpdatedDate,
                    secondDateName: nameof(User.UpdatedDate)),

                    Parameter: nameof(User.CreatedDate)));
        }

        private void ValidateUserOnModify(User user)
        {
            ValidateUserNotNull(user);

            //Validate(
            //    (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
            //    (Rule: IsInvalid(user.FirstName), Parameter: nameof(User.FirstName)),
            //    (Rule: IsInvalid(user.LastName), Parameter: nameof(User.LastName)),
            //    (Rule: IsInvalid(user.Email), Parameter: nameof(User.Email)),
            //    (Rule: IsInvalid(user.BirthDate), Parameter: nameof(User.BirthDate)),
            //    (Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
            //    (Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)),
            //    (Rule: IsInvalid(user.Password), Parameter: nameof(User.Password)),

            //    (Rule: IsSame(
            //        firstDate: user.UpdatedDate,
            //        secondDate: user.CreatedDate,
            //        secondDateName: nameof(User.CreatedDate)),

            //        Parameter: nameof(User.UpdatedDate)),

            //    (Rule: IsNotRecent(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)));
        }
        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = this.dateTimeBroker.GetCurrentDateTime();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }
        private static dynamic IsNotSame(
           DateTimeOffset firstDate,
           DateTimeOffset secondDate,
           string secondDateName) => new
           {
               Condition = firstDate != secondDate,
               Message = $"Date is not same as {secondDateName}"
           };

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Value is required"
        };

        private static void ValidateUserNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException();
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = new InvalidUserException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserException.ThrowIfContainsErrors();
        }

    }
}
