namespace Application.Options;

public sealed class BankingOptions
{
  public decimal DailyDebitLimit { get; set; } = 1000m;
}
