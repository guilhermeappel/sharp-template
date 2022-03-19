using Appel.SharpTemplate.API.Application.Extensions;
using Appel.SharpTemplate.API.Application.Models;
using Appel.SharpTemplate.Domain.Interfaces;
using Appel.SharpTemplate.Infrastructure.Application;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Validators.Models;

public class UserAuthenticateViewModelValidator : AbstractValidator<UserAuthenticateViewModel>
{
    private readonly IUserRepository _repository;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public UserAuthenticateViewModelValidator(IUserRepository repository, IOptionsMonitor<AppSettings> appSettings)
    {
        _repository = repository;
        _appSettings = appSettings;

        RuleFor(x => x)
            .MustAsync(BeValidUserAsync).OverridePropertyName("invalidLogin").WithMessage("Invalid e-mail or password");
    }

    public async Task<bool> BeValidUserAsync(UserAuthenticateViewModel user, CancellationToken cancellationToken)
    {
        var argon2HashManager = new Argon2HashManager(_appSettings);

        var databaseUser = (await _repository.GetAsync(x => x.Email == user.Email)).FirstOrDefault();

        return databaseUser != null && argon2HashManager.VerifyPasswordHash(user.Password, databaseUser.Password);
    }
}
