// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

String path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
Directory.SetCurrentDirectory(path);

while(true)
{
    Console.Write(Directory.GetCurrentDirectory() + ">");
    String command = Console.ReadLine().Trim();
    if(String.IsNullOrWhiteSpace(command))
    {
        continue;
    }
    String[] tokenized = command.Split(" ");
    if (String.Equals(tokenized[0], "exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
    else if (String.Equals(tokenized[0], "cd", StringComparison.OrdinalIgnoreCase) || String.Equals(tokenized[0], "chdir", StringComparison.OrdinalIgnoreCase))
    {
        if(tokenized.Length < 2)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
        }
        else
        {
            try
            {
                Directory.SetCurrentDirectory(tokenized[1]);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine("Error, System could not find the specified directory");
                Console.WriteLine("Details: " + ex.Message);
            }
        } 
    }
    else if (String.Equals(tokenized[0], "cls", StringComparison.OrdinalIgnoreCase) || String.Equals(tokenized[0], "clear", StringComparison.OrdinalIgnoreCase))
    {
        Console.Clear();
    }
    else if (String.Equals(tokenized[0], "dir", StringComparison.OrdinalIgnoreCase) || String.Equals(tokenized[0], "ls", StringComparison.OrdinalIgnoreCase))
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
            Console.ResetColor();
        }
    }
    else
    {
        var process = new Process();
        process.StartInfo.FileName = tokenized[0];
        process.StartInfo.UseShellExecute = false;  // must be false to avoid new window but still share console
        process.StartInfo.RedirectStandardInput = false;
        process.StartInfo.RedirectStandardOutput = false;
        process.StartInfo.RedirectStandardError = false;
        process.StartInfo.CreateNoWindow = false;   // show console window (shared)
        try
        {
            process.Start();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
            Console.ResetColor();
            continue;
        }



        //Console.WriteLine("unknown command: " + tokenized[0]);
    }

}
