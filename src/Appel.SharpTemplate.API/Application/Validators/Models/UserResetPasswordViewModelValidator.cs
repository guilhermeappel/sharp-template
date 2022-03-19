using Appel.SharpTemplate.API.Application.Extensions;
using Appel.SharpTemplate.API.Application.Models;
using Appel.SharpTemplate.API.Application.Validators.Extensions;
using Appel.SharpTemplate.Domain.Interfaces;
using Appel.SharpTemplate.Infrastructure.Application;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Validators.Models;

public class UserResetPasswordViewModelValidator : AbstractValidator<UserResetPasswordViewModel>
{
    private readonly IUserRepository _repository;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public UserResetPasswordViewModelValidator(IUserRepository repository, IOptionsMonitor<AppSettings> appSettings)
    {
        _repository = repository;
        _appSettings = appSettings;

        RuleFor(x => x)
            .MustAsync(BeValidHashAsync).WithMessage("Invalid e-mail hash");

        RuleFor(x => x.Password)
            .PasswordValidation();

        RuleFor(x => x.PasswordConfirmation)
            .IsRequired()
            .Equal(x => x.Password).WithMessage("Passwords doesn't match");
    }


    public async Task<bool> BeValidHashAsync(UserResetPasswordViewModel user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(user.EmailHash))
        {
            return false;
        }

        var databaseUser = await _repository.GetByIdAsync(user.Id, cancellationToken);

        if (databaseUser == null)
        {
            return false;
        }

        var decryptedData = CryptographyExtensions.Decrypt(_appSettings.CurrentValue.EmailTokenSecretKey, user.EmailHash);
        var emailToken = JsonSerializer.Deserialize<EmailTokenViewModel>(decryptedData);

        return emailToken?.Email == databaseUser.Email && emailToken?.Validity >= DateTime.Now;
    }
}
