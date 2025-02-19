namespace TransData.App.Interfaces;

public interface IColumnAction
{
  string FactoryIdentifier { get; }
  string Name { get; }
  string ConfigurationJson { get; set; }
  string Transform(string rawData);
}
