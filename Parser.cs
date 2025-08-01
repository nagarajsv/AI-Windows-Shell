namespace ConsoleApp2
{
	public class Parser
	{
		private List<Token> _tokens;
		private int _position;

		public Parser(List<Token> tokens)
		{
			_tokens = tokens;
			_position = 0;
		}

		public ASTNode Parse()
		{
			var commands = new List<CommandNode>();

			while(_position < _tokens.Count)
			{
				commands.Add(parseCommand());

				if(check(TokenType.Operator, "|"))
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

		private CommandNode parseCommand()
		{
			Token nameToken = consume(TokenType.Identifier, "Expected command name");
			string name = nameToken.Value;

			var args = new List<ArgumentNode>();
			
			while(_position < _tokens.Count && !check(TokenType.Operator, "|"))
			{
				if(check(TokenType.Operator, ">", ">>", "<"))
				{
					string op = _tokens[_position].Value;
					_position++;
					string target = consumeAny(TokenType.String, TokenType.Identifier).Value;
					args.Add(new RedirectionNode(op, target));
				}
				else if (check(TokenType.Flag))
				{
					string flag = _tokens[_position].Value;

					if(check(TokenType.Operator, "="))
					{
						_position++;
						string value = consumeAny(TokenType.String, TokenType.Identifier).Value;
						args.Add(new FlagNode(flag, value));
					}
				}
				else if (check(TokenType.Identifier) || check(TokenType.String))
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

		private Token consume(TokenType type, string errorMessage)
		{
			if(check(type))
			{
				Token token = _tokens[_position];
				_position++;
				return token;
			}

			throw new Exception(errorMessage);
		}

		private bool check(TokenType type, params string[] values)
		{
			if (_position >= _tokens.Count)
			{
				return false;
			}

			var token = _tokens[_position];

			return token.Type == type && (values.Length == 0 || values.Contains(token.Value));
		}

		private Token consumeAny(params TokenType[] types)
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