using FluentValidation;

namespace LocalCRM.Application.Companies;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(v => v.Name).MaximumLength(200).NotEmpty();
        RuleFor(v => v.CompanyRef).MaximumLength(50).NotEmpty();
        RuleFor(v => v.City).NotEmpty();
        RuleFor(v => v.CompanyType).NotEmpty();
    }
}
