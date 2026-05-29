using FluentValidation;
using HolidayApp.Features.Holidays.Shared;

namespace HolidayApp.Features.Holidays.GetCommonHolidays;

public class GetCommonHolidaysRequestValidator : AbstractValidator<GetCommonHolidaysRequest>
{
    public GetCommonHolidaysRequestValidator()
    {
        RuleFor(x => x.CountryCode1).MustBeValidCountryCode();
        RuleFor(x => x.CountryCode2).MustBeValidCountryCode();
        RuleFor(x => x.Year).MustBeValidYear();
        RuleFor(x => x)
            .Must(x => !x.CountryCode1.Equals(x.CountryCode2, StringComparison.OrdinalIgnoreCase))
            .WithMessage("CountryCode1 and CountryCode2 must be different.")
            .When(x => !string.IsNullOrEmpty(x.CountryCode1) && !string.IsNullOrEmpty(x.CountryCode2));
    }
}