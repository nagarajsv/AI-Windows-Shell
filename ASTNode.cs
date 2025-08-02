namespace ConsoleApp2
{
    public abstract class  ASTNode { }

    public class CommandNode(string name, List<ArgumentNode> args) : ASTNode
    {
        public string CommandName { get; } = name;
        public List<ArgumentNode> Arguments { get; } = args;
    }

    public class PipeNode(List<CommandNode> cmds) : ASTNode
    {
        public List<CommandNode> Commands { get; } = cmds;
    }

    public abstract class  ArgumentNode: ASTNode { }

    public class  StringNode(string val) : ArgumentNode
    {
        public string Value { get; } = val;
    }

    public class RedirectionNode(string opSymbol, string targetName) : ArgumentNode
    {
        public string Op { get; } = opSymbol;
        public string Target { get; } = targetName;
    }

    public class  FlagNode(string flagName, string? flagValue) : ArgumentNode
    {
        public string Name { get; } = flagName;
        public string? Value { get; } = flagValue;
    }
}