using System;
using System.Globalization;
class Program {
    static void Main() {
        decimal d = 100.0m;
        Console.WriteLine(d.ToString(CultureInfo.GetCultureInfo("id-ID")));
        Console.WriteLine(d.ToString(CultureInfo.GetCultureInfo("en-US")));
    }
}
