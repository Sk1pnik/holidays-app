using FluentValidation;
using HolidayApp.Features.Holidays.Shared;

namespace HolidayApp.Features.Holidays.SyncHolidays;

public sealed class SyncHolidaysRequestValidator : AbstractValidator<SyncHolidaysRequest>
{
    public SyncHolidaysRequestValidator()
    {
        RuleFor(x => x.CountryCode).MustBeValidCountryCode();
        RuleFor(x => x.Year).MustBeValidYear();
    }
}