using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Interpreter (ASTNode node)
    {
        private readonly ASTNode _node = node;

        public void Execute()
        {
            if(_node is CommandNode commandNode)
            {
                Console.WriteLine($"{commandNode.CommandName}");
                foreach (var arg in commandNode.Arguments)
                {
                    if (arg is StringNode stringArg)
                    {
                        Console.WriteLine(stringArg.Value);
                    }
                    else if(arg is FlagNode flagArg)
                    {
                        Console.Write(flagArg.Name + "=");
                        if(flagArg.Value != null)
                        {
                            Console.WriteLine(flagArg.Value);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Pipe Node");
            }
        }
    }
}
