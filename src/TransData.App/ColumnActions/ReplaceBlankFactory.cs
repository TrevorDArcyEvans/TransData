using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceBlankFactory : IColumnActionFactory
{
  public string Identifier { get; } = "ReplaceBlank";
  public string Name { get; } = "Replace Blank";
  public string Description { get; } = "Replace blank with fixed text";

  public IColumnAction Create(string configuration)
  {
    return new ReplaceBlank(configuration);
  }
}
