using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class TrimTextFactory : IColumnActionFactory
{
  public static string FactoryIdentifier = "TrimText";

  public string Identifier { get; } = FactoryIdentifier;
  public string Name { get; } = "Trim Text";
  public string Description { get; } = "Remove leading and trailing whitespace";

  public IColumnAction Create(string configuration)
  {
    return new TrimText();
  }
}
