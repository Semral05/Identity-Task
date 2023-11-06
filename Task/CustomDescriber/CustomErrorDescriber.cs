using Microsoft.AspNetCore.Identity;

namespace Task.CustomDescriber
{
    public class CustomErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new()
            {
                Code = "PasswordTooShort",
                Description=$"Password must be at least {length} characters"
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new()
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = "The password must contain at least one alphanumeric (~! etc.) character"
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new()
            {
                Code = "DuplicateUserName",
                Description = $"{userName} has already been taken"
            };
        }
    }
}
