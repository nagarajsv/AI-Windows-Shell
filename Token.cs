namespace ConsoleApp2
{
    public class Token(TokenType type, string value, int position)
    {
        public TokenType Type { get; } = type;
        public string Value { get; } = value;
        public int Position { get; } = position;

        public override string ToString()
        {
            return $"{Type}: '{Value}' at {Position}";
        }
    }
}