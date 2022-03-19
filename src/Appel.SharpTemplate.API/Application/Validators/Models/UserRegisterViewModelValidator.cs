using Appel.SharpTemplate.API.Application.Extensions;
using Appel.SharpTemplate.API.Application.Models;
using Appel.SharpTemplate.API.Application.Validators.Extensions;
using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using FluentValidation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Validators.Models;

public class UserRegisterViewModelValidator : AbstractValidator<UserRegisterViewModel>
{
    private readonly IUserRepository _repository;

    public UserRegisterViewModelValidator(IUserRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Name)
            .IsRequired();

        RuleFor(x => x.Email)
            .IsRequired()
            .EmailAddress().WithMessage("Invalid email address")
            .MustAsync(BeUniqueEmailAsync).WithMessage("E-mail already registered");

        RuleFor(x => x.Password)
            .PasswordValidation();

        RuleFor(x => x.PasswordConfirmation)
            .IsRequired()
            .Equal(x => x.Password).WithMessage("Passwords doesn't match");

        When(x => x.Type == UserType.Legal, () =>
        {
            RuleFor(x => x.CpfCnpj)
            .IsRequired()
            .Must(BeValidCnpj).WithMessage("Invalid CNPJ")
            .MustAsync(BeUniqueCpfCnpjAsync).WithMessage("CNPJ already registered");

            RuleFor(x => x.ResponsibleCpf)
            .IsRequired();

            RuleFor(x => x.ResponsibleName)
            .IsRequired();

            RuleFor(x => x.StateRegistration)
            .IsRequired();
        }).Otherwise(() =>
        {
            RuleFor(x => x.CpfCnpj)
            .IsRequired()
            .Must(BeValidCpf).WithMessage("Invalid CPF")
            .MustAsync(BeUniqueCpfCnpjAsync).WithMessage("CPF already registered");

            RuleFor(x => x.IdentityDocument)
            .IsRequired();
        });

        RuleFor(x => x.CellPhone)
            .IsRequired();

        RuleFor(x => x.Address)
            .IsRequired();

        RuleFor(x => x.AddressNumber)
            .IsRequired();

        RuleFor(x => x.Neighborhood)
            .IsRequired();

        RuleFor(x => x.City)
            .IsRequired();

        RuleFor(x => x.ZipCode)
            .IsRequired();

        RuleFor(x => x.FederativeUnit)
            .IsRequired();
    }

    public async Task<bool> BeUniqueEmailAsync(string email, CancellationToken cancellationToken)
    {
        return !(await _repository.GetAsync(filter: x => x.Email == email, cancellationToken: cancellationToken)).Any();

    }

    public async Task<bool> BeUniqueCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken)
    {
        return !(await _repository.GetAsync(filter: x => x.CpfCnpj == cpfCnpj, cancellationToken: cancellationToken)).Any();
    }

    public bool BeValidCpf(string cpf)
    {
        cpf = cpf.ToNumbersOnly();

        if (cpf.Length != 11)
        {
            return false;
        }

        // Test sequences like "99999999999"
        if (cpf.ToCharArray().All(x => x == cpf[0]))
        {
            return false;
        }

        string cpfTemp, checkDigits;
        var sum = 0;

        cpfTemp = cpf.Substring(0, 9);

        // Get first digit
        var multiplier1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        for (var i = 0; i < 9; i++)
        {
            sum += int.Parse(cpfTemp[i].ToString()) * multiplier1[i];
        }

        var mod = sum % 11;

        checkDigits = (mod < 2 ? 0 : 11 - mod).ToString();

        cpfTemp += checkDigits;

        sum = 0;

        // Get second digit
        var multiplier2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        for (var i = 0; i < 10; i++)
        {
            sum += int.Parse(cpfTemp[i].ToString()) * multiplier2[i];
        }

        mod = sum % 11;

        checkDigits += (mod < 2 ? 0 : 11 - mod).ToString();

        return cpf.EndsWith(checkDigits);
    }

    public bool BeValidCnpj(string cnpj)
    {
        cnpj = cnpj.ToNumbersOnly();

        if (cnpj.Length != 14)
        {
            return false;
        }

        // Test sequences like "99999999999"
        if (cnpj.ToCharArray().All(x => x == cnpj[0]))
        {
            return false;
        }

        int sum = 0, mod;

        var cnpjTemp = cnpj.Substring(0, 12);

        // Get first digit
        var multiplier1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        for (var i = 0; i < 12; i++)
        {
            sum += int.Parse(cnpjTemp[i].ToString()) * multiplier1[i];
        }

        mod = sum % 11;

        var checkDigits = (mod < 2 ? 0 : 11 - mod).ToString();

        cnpjTemp += checkDigits;

        sum = 0;

        // Get second digit
        var multiplier2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        for (var i = 0; i < 13; i++)
        {
            sum += int.Parse(cnpjTemp[i].ToString()) * multiplier2[i];
        }

        mod = sum % 11;

        checkDigits += (mod < 2 ? 0 : 11 - mod).ToString();

        return cnpj.EndsWith(checkDigits);
    }
}
