using System;
using System.Collections.Generic;
namespace Geocoder.SmartSearch
{
	public class ParserNode
	{
		private ParserNodeTypes nodeType = ParserNodeTypes.NULL;
		private ParserNode parentNode;
		public string[] Tokens;
		private bool OmitToken = false;
		private readonly string[] joiners = new string[]
		{
			"del",
			"la",
			"el",
			"los",
			"san",
			"calle"
		};
		private readonly string[] doubleJoiners = new string[]
		{
			"y",
			"e"
		};
		private readonly string[] numberJoiners = new string[]
		{
			"de"
		};
		private readonly string[] anyJoiners = new string[]
		{
			"av"
		};
		public static ParsedDirection[] Empty
		{
			get
			{
				return new ParsedDirection[0];
			}
		}
		public ParserNodeTypes NodeType
		{
			get
			{
				return this.nodeType;
			}
			set
			{
				this.nodeType = value;
			}
		}
		public ParserNode ParentNode
		{
			get
			{
				return this.parentNode;
			}
			set
			{
				this.parentNode = value;
			}
		}
		public int NodeIndex
		{
			get
			{
				return (this.ParentNode == null) ? 0 : (this.ParentNode.NodeIndex + 1);
			}
		}
		public int NodeTypeIndex
		{
			get
			{
				return (this.ParentNode == null) ? 0 : ((this.ParentNode.NodeType == this.NodeType) ? (this.ParentNode.NodeTypeIndex + 1) : 0);
			}
		}
		public int TypeIndex
		{
			get
			{
				return (this.ParentNode == null) ? 0 : ((this.ParentNode.NodeType != this.NodeType) ? (this.ParentNode.TypeIndex + 1) : this.ParentNode.TypeIndex);
			}
		}
		public string TypePath
		{
			get
			{
				return (this.ParentNode == null) ? ParserNode.TypeKey(this.NodeType).ToString() : ((this.ParentNode.NodeType != this.NodeType) ? (this.ParentNode.TypePath + ParserNode.TypeKey(this.NodeType)) : this.ParentNode.TypePath);
			}
		}
		public ParserNode()
		{
		}
		public ParserNode(ParserNodeTypes nodeType, ParserNode parentNode)
		{
			this.NodeType = nodeType;
			this.ParentNode = parentNode;
		}
		public ParsedDirection[] ParseTokens(string[] tokens)
		{
			this.Tokens = tokens;
			ParsedDirection[] result;
			if (this.Tokens.Length == 0 || !this.IsValidByToken())
			{
				result = ParserNode.Empty;
			}
			else
			{
				if (this.Tokens.Length == 1)
				{
					result = this.CreateDirection(this.Tokens[0]);
				}
				else
				{
					string[] tokens2 = this.NextTokens();
					List<ParsedDirection> list = new List<ParsedDirection>();
					ParserNodeTypes[] array = (this.Tokens[1] == ",") ? new ParserNodeTypes[]
					{
						this.NodeType
					} : ProbsDic.GetPossibleNext(this);
					ParserNodeTypes[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						ParserNodeTypes parserNodeTypes = array2[i];
						ParserNode parserNode = new ParserNode(parserNodeTypes, this);
						ParsedDirection[] collection = parserNode.ParseTokens(tokens2);
						list.AddRange(collection);
					}
					if (!this.OmitToken)
					{
						this.ModifyDirections(list, this.Tokens[0]);
					}
					result = list.ToArray();
				}
			}
			return result;
		}
		protected ParsedDirection[] CreateDirection(string token)
		{
			return this.ModifyDirections(new ParsedDirection[]
			{
				new ParsedDirection()
			}, token);
		}
		protected ParsedDirection[] ModifyDirections(IEnumerable<ParsedDirection> dirs, string token)
		{
			int altura;
			bool flag = int.TryParse(token, out altura);
			foreach (ParsedDirection current in dirs)
			{
				switch (this.NodeType)
				{
				case ParserNodeTypes.Calle:
					current.Calle = token + " " + current.Calle;
					break;
				case ParserNodeTypes.Equina:
					current.Esquina = token + " " + current.Esquina;
					break;
				case ParserNodeTypes.Altura:
					if (flag)
					{
						current.Altura = altura;
					}
					break;
				case ParserNodeTypes.Localidad:
					current.Localidad = token + " " + current.Localidad;
					break;
				case ParserNodeTypes.Provincia:
					current.Provincia = token + " " + current.Provincia;
					break;
				}
				bool flag2 = this.ParentNode == null || (this.ParentNode != null && this.ParentNode.NodeType != this.NodeType);
				current.Probabilidad *= (flag2 ? ProbsDic.GetTypeProb(this) : ProbsDic.GetProb(this));
				if (flag && this.NodeType != ParserNodeTypes.Altura)
				{
					current.Probabilidad *= 0.1;
				}
				if (this.ParentNode != null && this.ParentNode.Tokens[0] == "y" && this.NodeType != ParserNodeTypes.Equina)
				{
					current.Probabilidad *= 0.2;
				}
			}
			return new List<ParsedDirection>(dirs).ToArray();
		}
		private string[] NextTokens()
		{
			string[] array = new string[this.Tokens.Length - 1];
			Array.Copy(this.Tokens, 1, array, 0, this.Tokens.Length - 1);
			return array;
		}
		public static char TypeKey(ParserNodeTypes type)
		{
			char result;
			switch (type)
			{
			case ParserNodeTypes.Calle:
				result = 'C';
				break;
			case ParserNodeTypes.Equina:
				result = 'E';
				break;
			case ParserNodeTypes.Altura:
				result = 'A';
				break;
			case ParserNodeTypes.Localidad:
				result = 'L';
				break;
			case ParserNodeTypes.Provincia:
				result = 'P';
				break;
			default:
				result = '0';
				break;
			}
			return result;
		}
		protected bool IsValidByToken()
		{
			string text = this.Tokens[0];
			bool result;
			if (text == ",")
			{
				this.OmitToken = true;
				result = true;
			}
			else
			{
				if (this.ParentNode == null)
				{
					result = true;
				}
				else
				{
					string text2 = this.ParentNode.Tokens[0];
					string token = (this.ParentNode.ParentNode != null) ? this.ParentNode.ParentNode.Tokens[0] : null;
					string text3 = (this.Tokens.Length > 1) ? this.Tokens[1] : null;
					ParserNodeTypes parserNodeTypes = this.ParentNode.NodeType;
					ParserNodeTypes parserNodeTypes2 = (this.ParentNode.ParentNode != null) ? this.ParentNode.ParentNode.NodeType : ParserNodeTypes.NULL;
					int num;
					bool flag = int.TryParse(text, out num);
					int num2;
					bool flag2 = int.TryParse(text2, out num2);
					bool flag3 = text2 == ",";
					bool flag4 = this.IsJoiner(text2);
					bool flag5 = this.IsJoiner(token);
					bool flag6 = parserNodeTypes != this.NodeType;
					bool flag7 = parserNodeTypes2 != parserNodeTypes;
					switch (this.NodeType)
					{
					case ParserNodeTypes.Calle:
						if (flag4 && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsDoubleJoiner(text) && flag6)
						{
							result = false;
							return result;
						}
						if (flag3 && !flag6)
						{
							result = false;
							return result;
						}
						if (flag7 && flag5)
						{
							result = false;
							return result;
						}
						if (flag2 && this.IsNumberJoiner(text) && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsJoinableAny(text2) && flag7 && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsJoinableAny(text2) && this.IsDoubleJoiner(text) && !flag6)
						{
							result = false;
							return result;
						}
						break;
					case ParserNodeTypes.Equina:
						if (text == "y" && flag6)
						{
							this.OmitToken = true;
						}
						else
						{
							if (text == "e" && flag6 && text3 != null && text3.Length > 1)
							{
								char c = char.ToLower(text3[0]);
								if (c == 'i' || c == 'y')
								{
									this.OmitToken = true;
								}
							}
							else
							{
								if (flag4 && flag6)
								{
									result = false;
									return result;
								}
								if (this.IsDoubleJoiner(text) && flag6)
								{
									result = false;
									return result;
								}
							}
						}
						if (flag3 && !flag6)
						{
							result = false;
							return result;
						}
						if (flag7 && flag5)
						{
							result = false;
							return result;
						}
						if (flag2 && this.IsNumberJoiner(text) && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsJoinableAny(text2) && flag7 && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsJoinableAny(text2) && this.IsDoubleJoiner(text) && !flag6)
						{
							result = false;
							return result;
						}
						break;
					case ParserNodeTypes.Altura:
						if (!flag)
						{
							result = false;
							return result;
						}
						if (this.IsNumberJoiner(text3))
						{
							result = false;
							return result;
						}
						break;
					case ParserNodeTypes.Localidad:
						if (flag4 && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsDoubleJoiner(text) && flag6)
						{
							result = false;
							return result;
						}
						if (flag3 && !flag6)
						{
							result = false;
							return result;
						}
						if (flag7 && flag5)
						{
							result = false;
							return result;
						}
						if (flag2 && this.IsNumberJoiner(text) && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsJoinableAny(text2) && flag7 && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsJoinableAny(text2) && this.IsDoubleJoiner(text) && !flag6)
						{
							result = false;
							return result;
						}
						break;
					case ParserNodeTypes.Provincia:
						if (flag)
						{
							result = false;
							return result;
						}
						if (flag4 && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsDoubleJoiner(text) && flag6)
						{
							result = false;
							return result;
						}
						if (flag3 && !flag6)
						{
							result = false;
							return result;
						}
						if (flag7 && flag5)
						{
							result = false;
							return result;
						}
						if (this.IsJoinableAny(text2) && flag7 && flag6)
						{
							result = false;
							return result;
						}
						if (this.IsJoinableAny(text2) && this.IsDoubleJoiner(text) && !flag6)
						{
							result = false;
							return result;
						}
						break;
					}
					result = true;
				}
			}
			return result;
		}
		private bool IsJoiner(string token)
		{
			bool result;
			if (this.IsNumberJoiner(token))
			{
				result = true;
			}
			else
			{
				if (this.IsDoubleJoiner(token))
				{
					result = true;
				}
				else
				{
					string[] array = this.joiners;
					for (int i = 0; i < array.Length; i++)
					{
						string b = array[i];
						if (token == b)
						{
							result = true;
							return result;
						}
					}
					result = false;
				}
			}
			return result;
		}
		private bool IsDoubleJoiner(string token)
		{
			string[] array = this.doubleJoiners;
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				string b = array[i];
				if (token == b)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private bool IsNumberJoiner(string token)
		{
			string[] array = this.numberJoiners;
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				string b = array[i];
				if (token == b)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private bool IsJoinableAny(string token)
		{
			string[] array = this.anyJoiners;
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				string b = array[i];
				if (token == b)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}
