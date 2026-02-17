using Domain.Entities;
using FluentValidation;

namespace Application.Features.Schools.Validations;

internal class UpdateSchoolRequestValidator : AbstractValidator<UpdateSchoolRequest>
{
    public UpdateSchoolRequestValidator(ISchoolService schoolService)
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .MustAsync(async (id, cancellationToken) 
                => await schoolService.GetByIdAsync(id, cancellationToken) is School schoolInDb && schoolInDb.Id == id)
            .WithMessage("School does not exist.");

        RuleFor(request => request.Name)
            .NotEmpty()
                .WithMessage("School name is required.")
            .MaximumLength(60);

        RuleFor(request => request.EstablishedDate)
            .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Established date cannot be in the future.");
    }
}
