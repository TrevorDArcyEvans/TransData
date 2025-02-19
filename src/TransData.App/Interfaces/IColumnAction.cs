namespace TransData.App.Interfaces;

public interface IColumnAction
{
  string Name { get; }
  string Configuration { get; set; }
  string Transform(string rawData);
}
