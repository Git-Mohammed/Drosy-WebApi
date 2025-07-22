namespace Drosy.Infrastructure.Helper.PasswordResetToken
{
    public static class PasswordResetTokenHelper
    {
        public static Drosy.Domain.Entities.PasswordResetToken CreateToken(int userId)
        {
            return new Domain.Entities.PasswordResetToken
            {
                UserId = userId,
                TokenString = Guid.NewGuid().ToString(),
                ExpirationDate = DateTime.Now.AddMinutes(30),
            };
        }
    }
}
