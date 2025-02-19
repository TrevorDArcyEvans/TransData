using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class TrimText : IColumnAction
{
  public string FactoryIdentifier { get; set; } = TrimTextFactory.FactoryIdentifier;
  public string Name { get; set; } = "Trim Text";

  public string Transform(string rawData)
  {
    return rawData.Trim();
  }
}
