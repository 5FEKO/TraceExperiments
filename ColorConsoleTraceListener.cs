using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace TraceExperiments
{
  public class ColorConsoleTraceListener: ConsoleTraceListener
  {
    private ConsoleColor color;
    private bool traceMethod;
    private Dictionary<string, ConsoleColor> colorMap;
      
    // usage: initializeData="Green, true"      
    public ColorConsoleTraceListener(string initializeData)
      : base()
    {      
      ConsoleColor color = Console.ForegroundColor;
      bool traceMethod = false;

      var parameters = initializeData.Split(',');
      if(parameters.Length > 0)
      {
        Enum.TryParse<ConsoleColor>(parameters[0].Trim(), true, out color);
      }
      if(parameters.Length > 1)
      {
        traceMethod = parameters[1].Trim().StartsWith("true", StringComparison.OrdinalIgnoreCase);
      }

      this.Initialize(color, traceMethod);
    }

    public ColorConsoleTraceListener(bool traceMethod)
      : this(ConsoleColor.Black, traceMethod)
    {
    }

    // if black is specified, behavior is to color consecutive sources with consecutive color enums
    public ColorConsoleTraceListener(ConsoleColor color = ConsoleColor.Black, bool traceMethod = false)
      :base()
    {
      this.Initialize(color, traceMethod);
    }

    private void Initialize(ConsoleColor color, bool traceMethod)
    {
      this.color = color;
      this.traceMethod = traceMethod;
      this.colorMap = new Dictionary<string,ConsoleColor>();
      //this.TraceOutputOptions |= TraceOptions.Timestamp;
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
    {
      TraceEvent(eventCache, source, eventType, id, "{0}", message);
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
    {
      var originalColor = Console.ForegroundColor;
      Console.ForegroundColor = this.GetColor(source);
      AppendMethod(ref format);
      base.TraceEvent(eventCache, source, eventType, id, format, args);            
      Console.ForegroundColor = originalColor;
    }

    private void AppendMethod(ref string format)
    {
      if (this.traceMethod)
      {
        var stackTrace = Environment.StackTrace.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        var token = "System.Diagnostics.TraceSource";
        var search = stackTrace.SkipWhile(x => !x.Contains(token));
        var frame = search.SkipWhile(x => x.Contains(token)).First();
        var methodDetails = frame.Split(new string[] { " in " }, StringSplitOptions.RemoveEmptyEntries);

        format = string.Format("{0}{1}{2}", format, Environment.NewLine, methodDetails[0]);
        //format = string.Format("{0}.{1}\n\t{2}", method.ReflectedType.FullName, method.Name, format);
      }
    }

    private ConsoleColor GetColor (string source)
    {
      if(this.color == ConsoleColor.Black)
      {
        if (!colorMap.ContainsKey(source))
        {
          var numColors = Enum.GetNames(typeof(ConsoleColor)).Length;
          var maxUsedColor = colorMap.FirstOrDefault(x => x.Value == colorMap.Values.Max()).Value;
          var nextColorIndex = (short)maxUsedColor % (numColors - 1) + 1;  // skip black foreground not to use invisible ink.
          colorMap[source] = (ConsoleColor)nextColorIndex;
        }
        return colorMap[source];
      }
      else
      {
        return this.color;
      }
    }
  }
}
