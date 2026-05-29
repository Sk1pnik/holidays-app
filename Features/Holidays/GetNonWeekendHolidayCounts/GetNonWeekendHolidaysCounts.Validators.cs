using FluentValidation;
using HolidayApp.Features.Holidays.Shared;

namespace HolidayApp.Features.Holidays.GetNonWeekendHolidayCounts;

public sealed class GetNonWeekendHolidayCountsRequestValidator : AbstractValidator<GetNonWeekendHolidayCountsRequest>
{
    public GetNonWeekendHolidayCountsRequestValidator()
    {
        RuleFor(x => x.CountryCodes)
            .NotEmpty()
            .WithMessage("At least one country code is required.");
        RuleForEach(x => x.CountryCodes).MustBeValidCountryCode();
        RuleFor(x => x.Year).MustBeValidYear();
    }
}