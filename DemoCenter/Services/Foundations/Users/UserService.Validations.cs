using DemoCenter.Models.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;

namespace DemoCenter.Services.Foundations.Users
{
    public partial class UserService
    {

        private void ValidateUserOnAdd(User user)
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
            //    (Rule: IsNotRecent(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
            //    (Rule: IsInvalid(user.Password), Parameter: nameof(User.Password)),

            //    (Rule: IsNotSame(
            //        firstDate: user.CreatedDate,
            //        secondDate: user.UpdatedDate,
            //        secondDateName: nameof(User.UpdatedDate)),

            //        Parameter: nameof(User.CreatedDate)));
        }

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
