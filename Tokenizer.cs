using System;
using System.Security.Principal;

namespace ConsoleApp2
{
	public class Tokenizer(string input)
    {
		private readonly string _input = input;
		private int _position = 0;

        public List<Token> Tokenize()
		{
			List<Token> tokens = [];
			while(_position < _input.Length)
			{
				char currentChar = CurrentChar();
                if (char.IsWhiteSpace(currentChar))
                {
					ReadWhiteSpace();
                }
				else if (currentChar.Equals('"'))
				{
					tokens.Add(ReadString());
				}
				else if (char.IsDigit(currentChar)) 
				{
					tokens.Add(ReadNumber());
				}
				else if (currentChar.Equals('-'))
				{
					tokens.Add(ReadFlag());
				}
				else if ("|<>&=".Contains(currentChar))
				{
					tokens.Add(ReadOperator());
				}
				else if ("(){};".Contains(currentChar))
				{
					tokens.Add(new Token(TokenType.Punctuator, currentChar.ToString(), _position));
					_position++;
				}
				else if(char.IsLetter(currentChar))
				{
					tokens.Add(ReadIdentifier());
				}
				else
				{
					tokens.Add(new Token(TokenType.Unknown, currentChar.ToString(), _position));
					_position++;
				}
            }

			return tokens;
		}

		/**
		 * returns the next character from the input with the specified offset
		 * (default 0) from the string without consuming it
		 */
		private char CurrentChar(int offset = 0)
		{
			int index = _position + offset;
			if(index < 0 || index >= _input.Length)
			{
				return '\0';
			} 
			else
			{
				return _input[index];
			}
		}

		private void ReadWhiteSpace()
		{
            while (char.IsWhiteSpace(CurrentChar()))
            {
                _position++;
            }
            //return new Token(TokenType.Whitespace, _input.Substring(start, _position - start), start);
        }

		private Token ReadString()
		{
			_position++; //Consume iniial quote
			int start = _position;
			
			while(_position < _input.Length && CurrentChar() != '"')
			{
				_position++;
			}

			var returnVal = new Token(TokenType.String, _input[start.._position], start);
			_position++; // Consume closing quote
			return returnVal;
		}

		private Token ReadNumber()
		{
			int start = _position;
			while(_position < _input.Length && char.IsDigit(CurrentChar()))
			{
				_position++;
			}
			return new Token(TokenType.Number, _input[start.._position], start);
		}

		private Token ReadFlag()
		{
			_position++;
			while(_position < _input.Length && CurrentChar().Equals('-'))
			{
				_position++;
			}
			int start = _position;
			while(_position < _input.Length && char.IsLetterOrDigit(CurrentChar()))
			{
				_position++;
			}
			return new Token(TokenType.Flag, _input[start.._position], start);
		}

		private Token ReadOperator()
		{
			int start = _position;
			char current = CurrentChar();
			_position++;
			if(_position < _input.Length && CurrentChar().Equals(current) && current == '>')
			{
				_position++;
			}
			return new Token(TokenType.Operator, _input[start.._position], start);
		}

		private Token ReadIdentifier()
		{
			int start = _position;
			char current = CurrentChar();
			while(_position < _input.Length && (char.IsLetterOrDigit(current) || current.Equals('-') || current.Equals(':')))
			{
				_position++;
				current = CurrentChar();
			}

			return new Token(TokenType.Identifier, _input[start.._position], start);
		}
	}
}
