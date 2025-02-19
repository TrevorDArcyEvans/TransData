using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceBlank : IColumnAction
{
  public string FactoryIdentifier { get; } = "ReplaceBlank";
  public string Name { get; } = "Replace Blank";
  public string Configuration { get; set; }

  public ReplaceBlank(string configuration)
  {
    Configuration = configuration;
  }

  public string Transform(string rawData)
  {
    return string.IsNullOrEmpty(rawData) ? "MISSING" : rawData;
  }
}
