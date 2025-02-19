namespace TransData.App.Interfaces;

public interface IColumnAction
{
  string FactoryIdentifier { get; }
  string Name { get; }
  string Transform(string rawData);
}
