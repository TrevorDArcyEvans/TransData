using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class FixedTextFactory : IColumnActionFactory
{
  public static string FactoryIdentifier = "FixedText";

  public string Identifier { get; } = FactoryIdentifier;
  public string Name { get; } = "Fixed Text";
  public string Description { get; } = "Replace all text with fixed text";

  public IColumnAction Create(string configuration)
  {
    return new FixedText(configuration);
  }
}
