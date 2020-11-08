// Remember to use full name because adding new using directives change line numbers

namespace Coverlet.Core.Samples.Tests
{
    public class SelectionStatements
    {
        public string SwitchCsharp8(object value) => 
            value switch
                {
                    int i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    uint ui => ui.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    short s => s.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    _ => throw new System.NotSupportedException(),
                };
    }
}
