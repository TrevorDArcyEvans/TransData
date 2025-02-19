using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceText : IColumnAction
{
  public string FactoryIdentifier { get; } = ReplaceTextFactory.FactoryIdentifier;
  public string Name { get; } = "Replace Text";
  public string ConfigurationJson { get; set; }

  public ReplaceText(string configurationJson)
  {
    ConfigurationJson = configurationJson;
  }

  public string Transform(string rawData)
  {
    // target text at:
    //    start
    //    end
    //    anywhere
    return "REPLACE";
  }

  private enum Location
  {
    Start,
    End,
    Anywhere
  }

  private record Configuration(string Targettext, string Text, Location Location);
}
