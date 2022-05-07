using System;
using System.Diagnostics;

public static partial class VirtualKeyboard
{
    static Process keyboardProcess;

    public static void ShowOnScreenKeyboard()
    {
        ProcessStartInfo processInfo = new ProcessStartInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\osk.exe");
        processInfo.UseShellExecute = true;
        processInfo.Verb = "open";

        keyboardProcess = Process.Start(processInfo);
    }

    public static void HideOnScreenKeyboard()
    {
        if (keyboardProcess != null)
        {
            if (!keyboardProcess.HasExited)
                keyboardProcess.Kill();

            keyboardProcess = null;
        }
    }
}
