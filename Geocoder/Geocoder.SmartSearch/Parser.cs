using System;
using System.Collections.Generic;

namespace Geocoder.SmartSearch
{
	public class Parser
	{
		public ParsedDirection[] ParseTokens(string str)
		{
			var array = SplitString(str);
			ParsedDirection[] result;
			if (array.Length == 0)
			{
				result = new ParsedDirection[0];
			}
			else
			{
				var list = new List<ParsedDirection>();
				var possibleNext = ProbsDic.GetPossibleNext(null);
				var array2 = possibleNext;
				for (var i = 0; i < array2.Length; i++)
				{
					var nodeType = array2[i];
					var parserNode = new ParserNode(nodeType, null);
					var collection = parserNode.ParseTokens(array);
					list.AddRange(collection);
				}
				var num = 0.0;
				for (var j = list.Count - 1; j >= 0; j--)
				{
					var parsedDirection = list[j];
					int num2;
					if (parsedDirection.Probabilidad == 0.0 || string.IsNullOrEmpty(parsedDirection.Calle) || (string.IsNullOrEmpty(parsedDirection.Esquina) && parsedDirection.Altura <= 0) || int.TryParse(parsedDirection.Calle, out num2) || int.TryParse(parsedDirection.Esquina, out num2) || int.TryParse(parsedDirection.Localidad, out num2))
					{
						list.RemoveAt(j);
					}
					else
					{
						num += parsedDirection.Probabilidad;
					}
				}
				foreach (var parsedDirection in list)
				{
					parsedDirection.Probabilidad /= num;
				}
				list.Sort(new ParsedDirectionComparer());
				result = list.GetRange(0, Math.Min(5, list.Count)).ToArray();
			}
			return result;
		}
		private static string[] SplitString(string str)
		{
			var list = new List<string>();
			var text = string.Empty;
			var i = 0;
			while (i < str.Length)
			{
				var c = str[i];
				var c2 = c;
				if (c2 <= '/')
				{
					switch (c2)
					{
					    case ' ': break;
					    case '!': goto IL_E0;
					    case '"': goto IL_B8;
					    default:
						    switch (c2)
						    {
						        case '(':
						        case ')':
						        case ',':
						        case '-':
						        case '/':
							        goto IL_B8;
						        case '*':
						        case '+':
							        goto IL_E0;
						        case '.':
							        break;
						        default:
							        goto IL_E0;
						    }
						break;
					}
					
                    if (!string.IsNullOrEmpty(text)) list.Add(text);
					text = string.Empty;
				}
				else
				{
					if (c2 == ';') goto IL_B8;

					switch (c2)
					{
					    case '[':
					    case ']':
						    goto IL_B8;
					    case '\\':
						    goto IL_E0;
					    default:
						    switch (c2)
						    {
						        case '{':
						        case '}':
							        goto IL_B8;
						        case '|':
							        goto IL_E0;
						        default:
							        goto IL_E0;
						    }
					}
				}
				IL_F4:
				i++;
				continue;
				IL_B8:
				if (!string.IsNullOrEmpty(text)) list.Add(text);
				list.Add(",");
				text = string.Empty;
				goto IL_F4;
				IL_E0:
				text += char.ToLower(c);
				goto IL_F4;
			}
			
            if (!string.IsNullOrEmpty(text)) list.Add(text);
			return list.ToArray();
		}
	}
}
