namespace ConsoleApp2
{
    public abstract class  ASTNode { }

    public class CommandNode : ASTNode
    {
        public string commandName { get; }
        public List<ArgumentNode> arguments { get; }

        public CommandNode(string name, List<ArgumentNode> args)
        {
            commandName = name;
            arguments = args;
        }
    }

    public class PipeNode : ASTNode
    {
        public List<CommandNode> commands { get; }

        public PipeNode(List<CommandNode> cmds)
        {
            commands = cmds;
        }
    }

    public abstract class  ArgumentNode: ASTNode { }

    public class  StringNode : ArgumentNode
    {
        public string value { get; }

        public StringNode(string val)
        {
            value = val;
        }
    }
    public class RedirectionNode : ArgumentNode
    {
        public string op { get; }
        public string target { get; }

        public RedirectionNode(string opSymbol, string targetName)
        {
            op = opSymbol;
            target = targetName;
        }
    }

    public class  FlagNode : ArgumentNode
    {
        public string name { get; }
        public string value { get; }

        public FlagNode(string flagName, string flagValue)
        {
            name = flagName;
            value = flagValue;
        }
    }
}