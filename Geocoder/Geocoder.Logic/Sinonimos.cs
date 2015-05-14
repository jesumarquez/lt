using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;
using System.Xml;

namespace Geocoder.Logic
{
	public class Sinonimos
	{
		private static Sinonimos instance = null;
		private List<FraseSinonimo> Frases = new List<FraseSinonimo>();
		private readonly XmlDocument Doc = new XmlDocument();
		private XmlNode Root = null;
		private readonly string Filename;
		private static readonly object SingletonLock = new object();
		public static Sinonimos Instance
		{
			get
			{
				object singletonLock;
				Monitor.Enter(singletonLock = Sinonimos.SingletonLock);
				Sinonimos result;
				try
				{
					Sinonimos arg_23_0;
					if ((arg_23_0 = Sinonimos.instance) == null)
					{
						arg_23_0 = (Sinonimos.instance = new Sinonimos());
					}
					result = arg_23_0;
				}
				finally
				{
					Monitor.Exit(singletonLock);
				}
				return result;
			}
		}
		private Sinonimos()
		{
			string text = ConfigurationManager.AppSettings["sinonimos"];
			if (!string.IsNullOrEmpty(text))
			{
				if (HttpContext.Current != null)
				{
					text = HttpContext.Current.Server.MapPath(text);
				}
				if (!File.Exists(text))
				{
					throw new ConfigurationErrorsException("No existe el archivo de sinonimos:\n" + text);
				}
				this.Filename = text;
				this.ParseXml();
			}
		}
		public Tokenizer GetSinonimo(Tokenizer tokens)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < tokens.Count; i++)
			{
				string item = tokens[i];
				bool flag = false;
				List<FraseSinonimo> posibles = this.GetPosibles(tokens, i);
				if (posibles.Count > 0)
				{
					for (int j = 0; j < posibles.Count; j++)
					{
						FraseSinonimo fraseSinonimo = posibles[j];
						if (this.IsMatch(fraseSinonimo, tokens, i))
						{
							list.AddRange(fraseSinonimo.Sinonimo);
							i += fraseSinonimo.Frase.Count;
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					list.Add(item);
				}
			}
			tokens.Tokens = list;
			return tokens;
		}
		internal bool IsMatch(FraseSinonimo posible, Tokenizer tokens, int index)
		{
			bool result;
			for (int i = 0; i < posible.Frase.Count; i++)
			{
				string a = posible.Frase[i];
				if (a != tokens[i + index])
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		internal List<FraseSinonimo> GetPosibles(Tokenizer tokens, int index)
		{
			List<FraseSinonimo> list = new List<FraseSinonimo>(5);
			foreach (FraseSinonimo current in this.Frases)
			{
				if (current.Frase.Count <= tokens.Count - index && !(current.Frase[0] != tokens[index]))
				{
					list.Add(current);
				}
			}
			return list;
		}
		private void ParseXml()
		{
			this.Frases = new List<FraseSinonimo>();
			this.Doc.Load(this.Filename);
			foreach (XmlNode xmlNode in this.Doc.ChildNodes)
			{
				if (xmlNode.Name.ToLower() == "sinonimos")
				{
					this.Root = xmlNode;
				}
			}
			if (this.Root == null)
			{
				throw new XmlException("No existe el nodo raiz 'sinonimos' ");
			}
			foreach (XmlNode xmlNode in this.Root.ChildNodes)
			{
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name.ToLower() == "sinonimo")
				{
					string text = null;
					string text2 = null;
					foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
					{
						if (xmlAttribute.Name.ToLower() == "palabra")
						{
							text = xmlAttribute.Value.ToLower().Trim();
						}
						else
						{
							if (xmlAttribute.Name.ToLower() == "equivalencia")
							{
								text2 = xmlAttribute.Value.ToLower().Trim();
							}
						}
					}
					if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
					{
						Tokenizer tokenizer = Tokenizer.FromString(text, 3, true);
						Tokenizer tokenizer2 = Tokenizer.FromString(text2, 3, true);
						FraseSinonimo fraseSinonimo = new FraseSinonimo();
						fraseSinonimo.SetNormalized(tokenizer.ToArray(), tokenizer2.ToArray());
						this.Frases.Add(fraseSinonimo);
					}
				}
			}
		}
		public bool Add(string palabra, string equivalencia)
		{
			string text = palabra.Trim().ToLower();
			string text2 = equivalencia.Trim().ToLower();
			Tokenizer tokenizer = Tokenizer.FromString(text, 3, true);
			Tokenizer tokenizer2 = Tokenizer.FromString(text2, 3, true);
			FraseSinonimo fraseSinonimo = new FraseSinonimo();
			fraseSinonimo.SetNormalized(tokenizer.ToArray(), tokenizer2.ToArray());
			this.Frases.Add(fraseSinonimo);
			XmlNode xmlNode = this.Doc.CreateElement("sinonimo");
			XmlAttribute xmlAttribute = this.Doc.CreateAttribute("palabra");
			xmlAttribute.Value = text.ToLower();
			XmlAttribute xmlAttribute2 = this.Doc.CreateAttribute("equivalencia");
			xmlAttribute2.Value = text2.ToLower();
			xmlNode.Attributes.Append(xmlAttribute);
			xmlNode.Attributes.Append(xmlAttribute2);
			this.Root.AppendChild(xmlNode);
			this.Doc.Save(this.Filename);
			return true;
		}
		public bool Remove(string palabra)
		{
			string text = palabra.Trim().ToLower();
			Tokenizer tokenizer = Tokenizer.FromString(text, 3, true);
			FraseSinonimo fraseSinonimo = new FraseSinonimo();
			fraseSinonimo.SetNormalized(tokenizer.ToArray(), new string[]
			{
				"a"
			});
			for (int i = 0; i < this.Frases.Count; i++)
			{
				FraseSinonimo fraseSinonimo2 = this.Frases[i];
				if (fraseSinonimo2.EqualsFrase(fraseSinonimo.Frase))
				{
					this.Frases.RemoveAt(i);
					break;
				}
			}
			XmlNode xmlNode = null;
			foreach (XmlNode xmlNode2 in this.Root.ChildNodes)
			{
				if (xmlNode2.NodeType == XmlNodeType.Element && xmlNode2.Name.ToLower() == "sinonimo")
				{
					foreach (XmlAttribute xmlAttribute in xmlNode2.Attributes)
					{
						if (xmlAttribute.Name.ToLower() == "palabra" && xmlAttribute.Value.ToLower().Trim() == text)
						{
							xmlNode = xmlNode2;
							break;
						}
					}
				}
				if (xmlNode != null)
				{
					break;
				}
			}
			if (xmlNode != null)
			{
				this.Root.RemoveChild(xmlNode);
			}
			this.Doc.Save(this.Filename);
			return true;
		}
	}
}
