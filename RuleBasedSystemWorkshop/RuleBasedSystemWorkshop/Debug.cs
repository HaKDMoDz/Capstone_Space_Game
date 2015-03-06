using System;
using System.Collections.Generic;
using System.Linq;

class Debug
{
    public static void Log(string _text)
    {
        PrintError("Log: " + _text);
    }

    public static void LogWarning(string _text)
    {
        PrintError("Warning: " + _text);
    }

    public static void LogError(string _text)
    {
        PrintError("Error: " + _text);
    }

    private static void PrintError(string _text)
    {
        Console.WriteLine(_text);
    }
}
