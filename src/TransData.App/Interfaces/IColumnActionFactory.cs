namespace TransData.App.Interfaces;

public interface IColumnActionFactory
{
  string Identifier { get; }
  string Name { get; }
  string Description { get; }
  IColumnAction Create(string configuration = null);
}
