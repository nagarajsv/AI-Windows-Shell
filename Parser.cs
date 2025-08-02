namespace ConsoleApp2
{
	public class Parser(List<Token> tokens)
    {
		private readonly List<Token> _tokens = tokens;
		private int _position = 0;

        public ASTNode Parse()
		{
			var commands = new List<CommandNode>();

			while(_position < _tokens.Count)
			{
				commands.Add(ParseCommand());

				if(Check(TokenType.Operator, "|"))
				{
					_position++;
					continue;
				}
				else
				{
					break;
				}
			}

			if(commands.Count == 1)
			{
				return commands[0];
			}
			else
			{
				return new PipeNode(commands);
			}
		}

		private CommandNode ParseCommand()
		{
			Token nameToken = Consume(TokenType.Identifier, "Expected command name");
			string name = nameToken.Value;

			var args = new List<ArgumentNode>();
			
			while(_position < _tokens.Count && !Check(TokenType.Operator, "|"))
			{
				if(Check(TokenType.Operator, ">", ">>", "<"))
				{
					string op = _tokens[_position].Value;
					_position++;
					string target = ConsumeAny(TokenType.String, TokenType.Identifier).Value;
					args.Add(new RedirectionNode(op, target));
				}
				else if (Check(TokenType.Flag))
				{
					string flag = _tokens[_position].Value;

					if(Check(TokenType.Operator, "="))
					{
						_position++;
						string value = ConsumeAny(TokenType.String, TokenType.Identifier).Value;
						args.Add(new FlagNode(flag, value));
					}
				}
				else if (Check(TokenType.Identifier) || Check(TokenType.String))
				{
					args.Add(new StringNode(_tokens[_position].Value));
					_position++;
				}
				else
				{
					_position++;
				}
			}

			return new CommandNode(name, args);
		}

		private Token Consume(TokenType type, string errorMessage)
		{
			if(Check(type))
			{
				Token token = _tokens[_position];
				_position++;
				return token;
			}

			throw new Exception(errorMessage);
		}

		private bool Check(TokenType type, params string[] values)
		{
			if (_position >= _tokens.Count)
			{
				return false;
			}

			var token = _tokens[_position];

			return token.Type == type && (values.Length == 0 || values.Contains(token.Value));
		}

		private Token ConsumeAny(params TokenType[] types)
		{
			if(_position < _tokens.Count && types.Contains(_tokens[_position].Type))
			{
				Token token = _tokens[_position];
				_position++;
				return token;
			}
			throw new Exception("Unexpected token");
		}
	}
}