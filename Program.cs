// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using static ConsoleApp2.Commands;


String path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
Directory.SetCurrentDirectory(path);
Process? shellInstance = null;


String? apiKey = null;


async Task LLMResponse(String prompt)
{
    if(apiKey == null || String.IsNullOrEmpty(prompt))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error: API key is not set or prompt is empty.");
        Console.ResetColor();
        return;
    }

    String url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
    var json = $@"
                    {{
                        ""contents"": [
                            {{
                                ""parts"": [
                                    {{
                                        ""text"": ""{prompt}""
                                    }}
                                ]
                            }}
                        ],
                        ""generationConfig"": {{
                            ""thinkingConfig"": {{
                                ""thinkingBudget"": 0
                            }}
                        }}
                    }}";

    using var client = new HttpClient();
    client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await client.PostAsync(url, content);
    string responseString = await response.Content.ReadAsStringAsync();

    using JsonDocument doc = JsonDocument.Parse(responseString);

    JsonElement root = doc.RootElement;
    var text = root
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString();

    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine();
    Console.WriteLine(text);
    Console.WriteLine();
    Console.ResetColor();
}

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    if(shellInstance != null && !shellInstance.HasExited)
    {
        Console.WriteLine();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("CTRL+C");
        Console.ResetColor();
    }
};

Console.WriteLine("Welcome to the AI Shell! This AI uses google gemini 2.5 flash. \nEnter your google gemini API key, or enter a blank line to not use AI features:");
string apiInput = Console.ReadLine();
if (!String.IsNullOrWhiteSpace(apiInput))
{
    apiKey = apiInput.Trim();
}
else
{
    apiKey = "AIzaSyDut5OtWxmGR8zUF7SqZESQph9DI70gKN0";
}

    while (true)
    {
        Console.Write(Directory.GetCurrentDirectory() + "$> ");
        String? input = Console.ReadLine();
        if (input == null)
        {
            //Console.WriteLine();
            continue;
        }
        String command = input.Trim();
        if (String.IsNullOrWhiteSpace(command))
        {
            continue;
        }
        String[] tokenized = command.Split(" ");
        if (String.Equals("ai", tokenized[0], StringComparison.OrdinalIgnoreCase))
        {
            await LLMResponse(command.Substring(3));
        }
        else if (String.Equals(tokenized[0], "exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }
        else if (String.Equals(tokenized[0], "cd", StringComparison.OrdinalIgnoreCase) || String.Equals(tokenized[0], "chdir", StringComparison.OrdinalIgnoreCase))
        {
            Cd(tokenized);
        }
        else if (String.Equals(tokenized[0], "cls", StringComparison.OrdinalIgnoreCase) || String.Equals(tokenized[0], "clear", StringComparison.OrdinalIgnoreCase))
        {
            Console.Clear();
        }
        else if (String.Equals(tokenized[0], "dir", StringComparison.OrdinalIgnoreCase) || String.Equals(tokenized[0], "ls", StringComparison.OrdinalIgnoreCase))
        {
            Dir();
        }
        else
        {
            shellInstance = CustomCommand(tokenized);
            try
            {
                shellInstance.Start();
                shellInstance.WaitForExit();
            }
            catch (Exception ex)
            {
                ErrorMessage("Error: " + ex.Message);
            }



            //Console.WriteLine("unknown command: " + tokenized[0]);
        }

    }
