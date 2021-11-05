using System;
using System.Diagnostics;
using System.Text;

namespace mis4200team2.Logging
{
  public interface ILogger
  {
    void Info(string message);
    void Info(string fmt, params object[] vars);
    void Info(Exception exception, string fmt, params object[] vars);

    void Warn(string message);
    void Warn(string fmt, params object[] vars);
    void Warn(Exception exception, string fmt, params object[] vars);

    void Error(string message);
    void Error(string exception, params object[] vars);
    void Error(Exception ex, string fmt, params object[] vars);

    void TraceApi(string componentName, string method, TimeSpan timespan);
    void TraceApi(string componentName, string method, TimeSpan timespan, string properties);
    void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars);
  }
  public class Logger : ILogger
  {
    public void Info(string message)
    {
      Trace.TraceInformation(message);
    }
    public void Info(string fmt, params object[] vars)
    {
      Trace.TraceInformation(fmt, vars);
    }
    public void Info(Exception exception, string fmt, params object[] vars)
    {
      Trace.TraceInformation(FormatExceptionMessage(exception, fmt, vars));
    }

    public void Warn(string message)
    {
      Trace.TraceWarning(message);
    }
    public void Warn(string fmt, params object[] vars)
    {
      Trace.TraceWarning(fmt, vars);
    }
    public void Warn(Exception exception, string fmt, params object[] vars)
    {
      Trace.TraceWarning(FormatExceptionMessage(exception, fmt, vars));
    }

    public void Error(string message)
    {
      Trace.TraceError(message);
    }
    public void Error(string fmt, params object[] vars)
    {
      Trace.TraceError(fmt, vars);
    }
    public void Error(Exception exception, string fmt, params object[] vars)
    {
      Trace.TraceError(FormatExceptionMessage(exception, fmt, vars));
    }

    public void TraceApi(string componentName, string method, TimeSpan timespan)
    {
      TraceApi(componentName, method, timespan, "");
    }

    public void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars)
    {
      TraceApi(componentName, method, timespan, string.Format(fmt, vars));
    }
    public void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
    {
      string message = String.Concat("Component:", componentName, ";Method:", method, ";Timespan:", timespan.ToString(), ";Properties:", properties);
      Trace.TraceInformation(message);
    }

    private static string FormatExceptionMessage(Exception exception, string fmt, object[] vars)
    {
      // Simple exception formatting: for a more comprehensive version see 
      // https://code.msdn.microsoft.com/windowsazure/Fix-It-app-for-Building-cdd80df4
      var sb = new StringBuilder();
      sb.Append(string.Format(fmt, vars));
      sb.Append(" Exception: ");
      sb.Append(exception.ToString());
      return sb.ToString();
    }
  }
}