using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net.Http;

static class TextGenerator
{
	public const string SentencesFilePath = @"";
	public static void GenerateText(string webAddress, string filePath = SentencesFilePath)
	{
		// ServicePointManager.Expect100Continue = true;
		// ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		HttpClient client = new HttpClient();
		string url = @"https://en.wikipedia.org/wiki/Pi";
		char[] html = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result.ToArray();
		
		html = html.Replace("\n", "");
		html = html.Replace("\r", "");
		html = HtmlParser.GetTagContent(html, "p", " ");
		html = HtmlParser.RemoveTagContent(html, "semantics", "");
		html = HtmlParser.RemoveTagContent(html, "style", "");
		html = HtmlParser.RemoveTag(html, "");
		
		foreach (char[][] entity in HtmlParser.htmlEntities)
		{
			if(entity[1].Length != 0) html = html.Replace(entity[1], entity[0]);
			html = html.Replace(entity[2], entity[0]);
		}
		for (int i = 6, j; i >= 2; i--)
		{
			char[] spaces = new char[i];
			for (j = 0; j < i; j++) spaces[j] = ' ';
			html = html.Replace(spaces, " ");
		}
	}
	public static void GenerateTextFromWikipedia(string webAddress, string filePath = SentencesFilePath)
	{
        
	}
	public static char[] GetText(string filePath = SentencesFilePath)
	{
		if(!File.Exists(SentencesFilePath))
		{
			Console.WriteLine("error: in TextGenerator.GetText(string) file [{0}] not found => empty char[] returned", filePath);
			return new char[0];
		}
		char[][] lines = File.ReadAllLines(filePath).Select(line => line.ToArray()).ToArray();
		Random random = new Random();
		int randomLine = random.Next(0, lines.Length - 3);
		char[] output = new char[lines[randomLine].Length + lines[randomLine + 1].Length + lines[randomLine + 2].Length + 2];
		for (int i = 0, index = 0; i < 3; i++)
		{
			for (int j = 0; j < lines[randomLine + i].Length; j++) output[index++] = lines[randomLine + i][j];
			if (index < output.Length) output[index++] = ' ';
		}
		return output;
	}
}