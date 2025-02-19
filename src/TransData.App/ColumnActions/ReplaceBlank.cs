using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceBlank : IColumnAction
{
  public string FactoryIdentifier { get; set; } = ReplaceBlankFactory.FactoryIdentifier;
  public string Name { get; set; } = "Replace Blank";
  public string Text { get; set; } = "MISSING";

  public string Transform(string rawData)
  {
    return string.IsNullOrEmpty(rawData) ? Text : rawData;
  }
}
