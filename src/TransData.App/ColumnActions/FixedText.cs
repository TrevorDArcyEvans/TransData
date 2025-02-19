using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class FixedText : IColumnAction
{
  public string FactoryIdentifier { get; set; } = FixedTextFactory.FactoryIdentifier;
  public string Name { get; set; } = "Fixed Text";
  public string Text { get; set; } = "FIXED";

  public string Transform(string rawData)
  {
    return Text;
  }
}
