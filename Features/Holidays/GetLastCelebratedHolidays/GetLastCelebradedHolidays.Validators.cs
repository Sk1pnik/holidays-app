using FluentValidation;
using HolidayApp.Features.Holidays.Shared;

namespace HolidayApp.Features.Holidays.GetLastCelebratedHolidays;

public sealed class GetLastCelebratedHolidaysRequestValidator : AbstractValidator<GetLastCelebratedHolidaysRequest>
{
    public GetLastCelebratedHolidaysRequestValidator()
    {
        RuleFor(x => x.CountryCode).MustBeValidCountryCode();
    }
}