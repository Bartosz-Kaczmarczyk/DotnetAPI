using DotnetAPI.Data;
using DotnetAPI.Dtos;
using FluentValidation;

namespace DotnetAPI.Models.Validators;

public class PasswordChangeValidator : AbstractValidator<PasswordChangeDto>
    {
    private readonly DataContextEF _entity;

    public PasswordChangeValidator(IConfiguration config)
        {
            _entity = new DataContextEF(config);

            RuleFor(x => x.Password).MinimumLength(6);

            RuleFor(x => x.PasswordConfirm).Equal(e => e.Password);
        }
    }