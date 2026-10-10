using System;
using System.Globalization;

class Program {
    static void Main() {
        decimal d = decimal.Parse("100.0", CultureInfo.InvariantCulture);
        Console.WriteLine(d.ToString(CultureInfo.GetCultureInfo("id-ID")));
    }
}
