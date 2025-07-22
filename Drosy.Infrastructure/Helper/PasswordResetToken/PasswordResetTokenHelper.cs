namespace Drosy.Infrastructure.Helper.PasswordResetToken
{
    public static class PasswordResetTokenHelper
    {
        public static Drosy.Domain.Entities.PasswordResetToken CreateToken(int userId, string toekn)
        {
            return new Domain.Entities.PasswordResetToken
            {
                UserId = userId,
                TokenString = toekn,
                ExpirationDate = DateTime.Now.AddMinutes(30),
            };
        }
    }
}
