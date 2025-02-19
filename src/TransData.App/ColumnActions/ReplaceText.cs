using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceText : IColumnAction
{
  public string FactoryIdentifier { get; } = ReplaceTextFactory.FactoryIdentifier;
  public string Name { get; } = "Replace Text";
  public string Configuration { get; set; }

  public ReplaceText(string configuration)
  {
    Configuration = configuration;
  }

  public string Transform(string rawData)
  {
    // target text at:
    //    start
    //    end
    //    anywhere
    return "REPLACE";
  }
}
