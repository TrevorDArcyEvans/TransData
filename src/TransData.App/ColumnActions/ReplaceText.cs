using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceText : IColumnAction
{
  public string FactoryIdentifier { get; set; } = ReplaceTextFactory.FactoryIdentifier;
  public string Name { get; set; } = "Replace Text";
  public string TargetText { get; set; } = "NA";
  public string Text { get; set; } = "REPLACE";
  public Location TextLocation { get; set; } = Location.Start;

  public string Transform(string rawData)
  {
    // target text at:
    //    start
    //    end
    //    anywhere
    return Text;
  }

  public enum Location
  {
    Start,
    End,
    Anywhere
  }
}
