using System.Text;

namespace Geocoder.Logic
{
	public static class Normalizer
	{
		public static string Normalizar(string text)
		{
			text = text.ToLower();
			text = Normalizer.RemueveAcentos(text);
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			char c = '\0';
			for (int i = 0; i < text.Length; i++)
			{
				char c2 = text[i];
				char c3 = (i + 1 < text.Length) ? text[i + 1] : '\0';
				if (c2 != c || c == 'l' || char.IsNumber(c))
				{
					char c4 = c2;
					if (c4 <= 'z')
					{
						switch (c4)
						{
						case '#':
							goto IL_304;
						case '$':
						case '%':
						case '&':
						case '(':
						case ')':
						case '*':
						case '+':
						case '-':
						case '/':
						case ':':
							goto IL_3C7;
						case '\'':
						case ',':
						case '.':
						case ';':
							break;
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							stringBuilder.Append(c2);
							break;
						default:
							switch (c4)
							{
							case 'c':
								if (c3 == 'e' || c3 == 'i')
								{
									stringBuilder.Append('s');
								}
								else
								{
									if (c3 == 'c')
									{
										stringBuilder.Append('x');
										i++;
									}
									else
									{
										stringBuilder.Append('k');
									}
								}
								break;
							case 'd':
							case 'e':
							case 'f':
								goto IL_3C7;
							case 'g':
								if (c3 == 'e' || c3 == 'i')
								{
									stringBuilder.Append('j');
								}
								else
								{
									stringBuilder.Append('g');
								}
								break;
							case 'h':
								if (text.Length == 1)
								{
									stringBuilder.Append('h');
								}
								else
								{
									if (c == 'k' || c == 'c')
									{
										stringBuilder.Append('h');
									}
								}
								break;
							case 'i':
								if (c != 'ñ')
								{
									stringBuilder.Append('i');
								}
								break;
							default:
								switch (c4)
								{
								case 'l':
									if (c == 'l')
									{
										if (c3 == 'a' || c3 == 'e' || c3 == 'i' || c3 == 'o' || c3 == 'u')
										{
											stringBuilder[stringBuilder.Length - 1] = 'y';
										}
									}
									else
									{
										stringBuilder.Append('l');
									}
									break;
								case 'm':
								case 'n':
								case 'o':
								case 'p':
								case 'r':
								case 't':
								case 'x':
									goto IL_3C7;
								case 'q':
									stringBuilder.Append('k');
									break;
								case 's':
									if (c3 == 'h')
									{
										stringBuilder.Append('y');
									}
									else
									{
										stringBuilder.Append('s');
									}
									break;
								case 'u':
									if ((c == 'g' || c == 'q') && (c3 == 'e' || c3 == 'i'))
									{
										if (c3 != 'e' && c3 != 'i')
										{
											stringBuilder.Append('u');
										}
									}
									else
									{
										stringBuilder.Append('u');
									}
									break;
								case 'v':
									stringBuilder.Append('b');
									break;
								case 'w':
									stringBuilder.Append('u');
									break;
								case 'y':
									if (c3 == 'a' || c3 == 'e' || c3 == 'i' || c3 == 'o' || c3 == 'u')
									{
										stringBuilder.Append('y');
									}
									else
									{
										stringBuilder.Append('i');
									}
									break;
								case 'z':
									stringBuilder.Append('s');
									break;
								default:
									goto IL_3C7;
								}
								break;
							}
							break;
						}
					}
					else
					{
						if (c4 != 'ª' && c4 != 'º')
						{
							if (c4 != 'ñ')
							{
								goto IL_3C7;
							}
							goto IL_304;
						}
					}
					IL_3D1:
					c = c2;
					goto IL_3D4;
					IL_304:
					stringBuilder.Append("ni");
					goto IL_3D1;
					IL_3C7:
					stringBuilder.Append(c2);
					goto IL_3D1;
				}
				IL_3D4:;
			}
			return stringBuilder.ToString();
		}
		private static string RemueveAcentos(string text)
		{
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			int i = 0;
			while (i < text.Length)
			{
				char value = text[i];
				switch (value)
				{
				case 'à':
				case 'á':
				case 'ã':
				case 'ä':
					stringBuilder.Append('a');
					break;
				case 'â':
				case 'å':
				case 'æ':
				case 'ç':
				case 'ê':
				case 'î':
				case 'ð':
				case 'ñ':
				case 'ô':
				case 'õ':
				case '÷':
				case 'ø':
				case 'û':
				case 'þ':
					goto IL_F1;
				case 'è':
				case 'é':
				case 'ë':
					stringBuilder.Append('e');
					break;
				case 'ì':
				case 'í':
				case 'ï':
					stringBuilder.Append('i');
					break;
				case 'ò':
				case 'ó':
				case 'ö':
					stringBuilder.Append('o');
					break;
				case 'ù':
				case 'ú':
				case 'ü':
					stringBuilder.Append('u');
					break;
				case 'ý':
				case 'ÿ':
					stringBuilder.Append('y');
					break;
				default:
					goto IL_F1;
				}
				IL_FB:
				i++;
				continue;
				IL_F1:
				stringBuilder.Append(value);
				goto IL_FB;
			}
			return stringBuilder.ToString();
		}
	}
}
