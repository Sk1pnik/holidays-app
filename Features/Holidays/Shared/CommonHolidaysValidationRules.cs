using FluentValidation;

namespace HolidayApp.Features.Holidays.Shared;

public static class CommonValidationRules
{
    public const int MinYear = 1900;
    public const int MaxYear = 2100;

    public static IRuleBuilderOptions<T, string> MustBeValidCountryCode<T>(
        this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty()
            .Length(2)
            .Matches("^[A-Za-z]{2}$")
            .WithMessage("'{PropertyName}' must be a 2-letter ISO 3166-1 alpha-2 code (e.g. US, DE).");

    public static IRuleBuilderOptions<T, int> MustBeValidYear<T>(
        this IRuleBuilder<T, int> rule) =>
        rule.InclusiveBetween(MinYear, MaxYear)
            .WithMessage($"'{{PropertyName}}' must be between {MinYear} and {MaxYear}.");
}