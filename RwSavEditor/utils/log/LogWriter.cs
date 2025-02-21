using RwSavEditor.utils.logs.logtypes;

namespace RwSavEditor.utils;

public class LogWriter
{
    public static void WriteLog(string message, AbstractLogType logType)
    {
        Console.ForegroundColor = logType.TextColor;
        Console.WriteLine(logType.Prefix + " -> " + message);
        Console.ResetColor();
    }
}