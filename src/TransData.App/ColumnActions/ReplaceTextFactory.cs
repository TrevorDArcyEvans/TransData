using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceTextFactory : IColumnActionFactory
{
  public static string FactoryIdentifier = "ReplaceText";

  public string Identifier { get; } = FactoryIdentifier;
  public string Name { get; } = "Replace Text";
  public string Description { get; } = "Replace specified text with other text";

  public IColumnAction Create(string configuration)
  {
    return new ReplaceText(configuration);
  }
}
