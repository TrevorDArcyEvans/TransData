using System;
using TransData.App.Interfaces;

namespace TransData.App.ColumnActions;

public class ReplaceText : IColumnAction
{
  public string FactoryIdentifier { get; set; } = ReplaceTextFactory.FactoryIdentifier;
  public string Name { get; set; } = "Replace Text";
  public string TargetText { get; set; } = "NA";
  public string Text { get; set; } = "REPLACE";
  public Location TextLocation { get; set; } = Location.Start;

  public string Transform(string rawData)
  {
    switch (TextLocation)
    {
      case Location.Start:
        if (rawData.StartsWith(TargetText))
        {
          return rawData.Replace(TargetText, Text);
        }

        break;

      case Location.End:
        if (rawData.EndsWith(TargetText))
        {
          return rawData.Replace(TargetText, Text);
        }

        break;

      case Location.Anywhere:
        if (rawData.Contains(TargetText))
        {
          return rawData.Replace(TargetText, Text);
        }

        break;

      default:
        throw new ArgumentOutOfRangeException();
    }

    return rawData;
  }

  public enum Location
  {
    Start,
    End,
    Anywhere
  }
}
