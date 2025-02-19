using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceBlank : IColumnAction
{
  public string FactoryIdentifier { get; } = ReplaceBlankFactory.FactoryIdentifier;
  public string Name { get; } = "Replace Blank";
  public string ConfigurationJson { get; set; }

  public ReplaceBlank(string configurationJson)
  {
    ConfigurationJson = configurationJson;
  }

  public string Transform(string rawData)
  {
    return string.IsNullOrEmpty(rawData) ? "MISSING" : rawData;
  }

  private record Configuration(string Text);
}
