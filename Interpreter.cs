using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Interpreter (ASTNode node)
    {
        private readonly ASTNode _node = node;
        private static readonly string[] HTTPMethods = ["GET", "POST", "PUT", "DELETE"];

        public async Task Execute()
        {
            if(_node is CommandNode commandNode)
            {
                switch (commandNode.CommandName.ToLower())
                {
                    case "cd":
                    case "chdir":
                    case "set-location":
                        Cd(commandNode);
                        break;
                    case "ls":
                    case "dir":
                    case "get-childitem":
                        Ls(commandNode);
                        break;
                    case "cls":
                    case "clear":
                    case "clear-host":
                        Console.Clear();
                        break;
                    case "curl":
                    case "invoke-webrequest":
                        await Curl(commandNode);
                        break;
                    case "exit":
                        break;

                }
                
            }
            else
            {
                Console.WriteLine("Pipe Node");
            }
        }

        public static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void Cd(CommandNode node)
        {
            if (node.Arguments.Count == 0)
            {
                Console.WriteLine(Directory.GetCurrentDirectory());
            }
            else if(node.Arguments.Count == 1 && node.Arguments[0].GetType() == typeof(StringNode))
            {
                try
                {
                    Directory.SetCurrentDirectory(((StringNode) node.Arguments[0]).Value);
                }
                catch (DirectoryNotFoundException ex)
                {
                    ErrorMessage("Error, System could not find the specified directory\nDetails: " + ex.Message);
                }
            }
            else
            {
                ErrorMessage("Error, only pass 0 or 1 arguments to Set-Location");
            }
        }

        private static void Ls(CommandNode node)
        {
            DirectoryInfo dirInfo;
            if (node.Arguments.Count == 0)
            {
                dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            }
            else if(node.Arguments.Count == 1 && node.Arguments[0].GetType() == typeof(StringNode))
            {
                try
                {
                    dirInfo = new DirectoryInfo(((StringNode)node.Arguments[0]).Value);
                }
                catch (DirectoryNotFoundException ex)
                {
                    ErrorMessage("Error, System could not find the specified directory.\nDetails: " + ex.Message);
                    return;
                }
                
            }
            else
            {
                ErrorMessage("Error, only pass 0 or 1 arguments to Get-ChildItem");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Directory: " + dirInfo.FullName);
            Console.WriteLine();

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
            Console.WriteLine();
        }

        private static async Task Curl(CommandNode node)
        {
            try
            {
                using var client = new HttpClient();
                string? url = null;
                string method = "GET";
                var headers = new Dictionary<string, string>();
                string? data = null;
                string? outputFile = null;
                bool verbose = false;

                foreach (var arg in node.Arguments)
                {
                    if(arg is StringNode stringNode)
                    {
                        if (url == null && Uri.IsWellFormedUriString(stringNode.Value, UriKind.Absolute))
                        {
                            url = stringNode.Value;
                        }
                        else
                        {
                            ErrorMessage("Error: Invalid URL or multiple URLs provided.");
                            return;
                        }
                    }
                    else if(arg is FlagNode flagNode)
                    {
                        switch(flagNode.Name)
                        {
                            case "X":
                                method = flagNode.Value?.ToUpper() ?? "GET";
                                if (!HTTPMethods.Contains(method))
                                {
                                    ErrorMessage("Unsupported HTTP method: " + method);
                                    return;
                                }
                                break;
                            case "H":
                                if(flagNode.Value != null)
                                {
                                    var parts = flagNode.Value.Split(':', 2);
                                    if(parts.Length == 2)
                                    {
                                        headers[parts[0].Trim()] = parts[1].Trim();
                                    }
                                    else
                                    {
                                        ErrorMessage("Invalud header format. Use 'Header: Value' format.");
                                        return;
                                    }
                                }
                                break;
                            case "d":
                                data = flagNode.Value;
                                break;
                            case "o":
                                outputFile = flagNode.Value;
                                break;
                            case "v":
                                verbose = true;
                                break;
                            default:
                                ErrorMessage("Unsupport flag: " + flagNode.Name);
                                return;
                        }
                    }
                    else if (arg is RedirectionNode redirNode && redirNode.Op == ">")
                    {
                        outputFile = redirNode.Target;
                    }
                    else
                    {
                        ErrorMessage("Unsupported argument type");
                        return;
                    }
                }

                if(String.IsNullOrEmpty(url))
                {
                    ErrorMessage("Error: No URL provided.");
                    return;
                }

                var req = new HttpRequestMessage(new HttpMethod(method), url);
                if(data != null)
                {
                    req.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }

                foreach (var header in headers)
                {
                    req.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;

                if(verbose)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Sending {method} request to {url}");
                    if(data != null)
                    {
                        Console.WriteLine($"Request Body: {data}");
                    }
                    Console.WriteLine("Headers:");
                    foreach (var header in headers)
                    {
                        Console.WriteLine($"    {header.Key}: {header.Value}");
                    }
                }

                HttpResponseMessage res = await client.SendAsync(req);

                string responseContent = await res.Content.ReadAsStringAsync();

                if (verbose)
                {
                    Console.WriteLine($"Response Status: {(int)res.StatusCode} {res.StatusCode}");
                    Console.WriteLine("Response Headers:");
                    foreach (var header in res.Headers)
                    {
                        Console.WriteLine($"    {header.Key}: {string.Join(", ", header.Value)}");
                    }
                    Console.WriteLine($"Response Body: {responseContent}");
                }
                else
                {
                    if (outputFile != null)
                    {
                        await File.WriteAllTextAsync(outputFile, responseContent);
                    }
                    else
                    {
                        Console.WriteLine($"Reponse Body:\n{responseContent}");
                    }

                    if(!res.IsSuccessStatusCode)
                    {
                        ErrorMessage($"Error: HTTP {(int) res.StatusCode} {res.StatusCode}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage("Error: " + ex.Message);
                return;
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
