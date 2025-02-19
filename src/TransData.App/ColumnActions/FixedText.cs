using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class FixedText : IColumnAction
{
  public string FactoryIdentifier { get; } = FixedTextFactory.FactoryIdentifier;
  public string Name { get; } = "Fixed Text";
  public string ConfigurationJson { get; set; }

  public FixedText(string configurationJson)
  {
    ConfigurationJson = configurationJson;
  }

  public string Transform(string rawData)
  {
    return "FIXED";
  }

  private record Configuration(string Text);
}
