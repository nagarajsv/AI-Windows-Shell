using System;
using System.Data;
using System.Diagnostics;

namespace ConsoleApp2
{
    public class Commands
    {
        public static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void Cd(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(Directory.GetCurrentDirectory());
            }
            else
            {
                try
                {
                    Directory.SetCurrentDirectory(args[1]);
                }
                catch (DirectoryNotFoundException ex)
                {
                    ErrorMessage("Error, System could not find the specified directory\nDetails: " + ex.Message);
                }
            }
        }

        public static void Dir()
        {
            try
            {
                var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (var file in dirInfo.GetFiles())
                {
                    Console.WriteLine(file.Name);
                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (var dir in dirInfo.GetDirectories())
                {
                    Console.WriteLine(dir.Name + "\\");
                }
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                ErrorMessage("Error: " + ex.Message);
            }
        }

        public static Process CustomCommand(string[] args)
        {
            var shellInstance = new Process();
            shellInstance.StartInfo.FileName = "powershell";
            shellInstance.StartInfo.Arguments = $"-Command \"{args[0]}\"";
            shellInstance.StartInfo.UseShellExecute = false;
            shellInstance.StartInfo.RedirectStandardInput = false;
            shellInstance.StartInfo.RedirectStandardOutput = false;
            shellInstance.StartInfo.RedirectStandardError = false;
            shellInstance.StartInfo.CreateNoWindow = false;

            return shellInstance;
        }



    }
}
