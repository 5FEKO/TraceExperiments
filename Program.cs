using System;
using System.Diagnostics;

namespace TraceExperiments
{
  class Program
  {
    static void Main(string[] args)
    {
      var methodWithAutoColorsListener = new ColorConsoleTraceListener(true);
      var magentaListener = new ColorConsoleTraceListener(ConsoleColor.DarkMagenta);

      for (int i = 0; i <20; i++)
      {
        var source = "source " + i;
        var trace = new TraceSource(source, SourceLevels.All);
        trace.Listeners.Add(methodWithAutoColorsListener);
        trace.TraceInformation("methodWithAutoColorsListener " + i);
      }
      for (int i = 0; i < 5; i++)
      {
        var source = "source " + i;
        var trace = new TraceSource(source, SourceLevels.All);
        trace.Listeners.Add(magentaListener);
        trace.TraceInformation("magentaListener " + i);
      }

      Console.ReadLine();
    }
  }
}
