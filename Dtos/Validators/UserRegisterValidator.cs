using DotnetAPI.Data;
using DotnetAPI.Dtos;
using FluentValidation;

namespace DotnetAPI.Models.Validators;

public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
    {
    private readonly DataContextEF _entity;

    public UserRegisterValidator(IConfiguration config)
        {
            _entity = new DataContextEF(config);

            RuleFor(x => x.Nick)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).MinimumLength(6);

            RuleFor(x => x.PasswordConfirm).Equal(e => e.Password);

            RuleFor(x => x.Nick)
                .Custom((value, context) =>
                {
                    var nickInUse = _entity.Users.Any(u => u.Nick == value);
                    if (nickInUse)
                    {
                        context.AddFailure("Nick", "That nick is taken");
                    }
                });

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = _entity.Users.Any(u => u.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });
        }
    }