using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class FixedText : IColumnAction
{
  public string FactoryIdentifier { get; } = FixedTextFactory.FactoryIdentifier;
  public string Name { get; } = "Fixed Text";
  public string Configuration { get; set; }

  public FixedText(string configuration)
  {
    Configuration = configuration;
  }

  public string Transform(string rawData)
  {
    return "FIXED";
  }
}
