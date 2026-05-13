

using Microsoft.AspNetCore.Identity;

namespace Dictionary.Persistence.Identity;
public class DictionaryIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = "Bu kullanıcı adı zaten kullanılıyor."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = "Bu e-posta adresi zaten kayıtlı."
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"Şifre en az {length} karakter olmalıdır."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Şifre en az bir büyük harf içermelidir."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Şifre en az bir küçük harf içermelidir."
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Şifre en az bir rakam içermelidir."
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Şifre en az bir özel karakter içermelidir."
        };
    }

    public override IdentityError InvalidEmail(string email)
    {
        return new IdentityError
        {

            Code = nameof(InvalidEmail),
            Description = $"Geçersiz e-posta adresi: {email}."
        };
    }
}
