using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;

public static class HtmlParser
{
	public const string HtmlEntitiesFilePath = @"..\data\html-enteties.csv";
	public readonly static char[][][] htmlEntities;
	static HtmlParser()
	{
		char[][] rows = File.ReadAllLines(HtmlEntitiesFilePath).Select(line => line.ToArray()).ToArray();
		htmlEntities = new char[rows.Length][][];
		for (int i = 0, start, end, index; i < rows.Length; i++)
		{
			htmlEntities[i] = new char[3][];
			for (index = 2, start = rows[i].Length, end = start; index >= 0; index--)
			{
				while(--start > 0 && rows[i][start - 1] != ',');
				htmlEntities[i][index] = new char[end - start];
				while(end-- != start) htmlEntities[i][index][end - start] = rows[i][end];
			}
		}
	}
	public static char[] MultiSubstring(this char[] input, List<int> indexToIndex, char[] divider, int increment, int startingIndex)
	{
		int length = -divider.Length;
		char[] output;
		for (int i = startingIndex; i + 1 < indexToIndex.Count(); i += increment)
			length += indexToIndex[i + 1] - indexToIndex[i] + 1) + divider.Length;
				
		for (int i = startingIndex; i + 1 < indexToIndex.Count(); i += increment)
		{
			if (indexToIndex[i] > indexToIndex[i + 1]) continue;
			if (indexToIndex[i] < 0 || indexToIndex[i + 1] >= input.Length)
				throw new Exception("Out Of Range");
			if (i != startingIndex) output += divider;
			output += input.Substring(indexToIndex[i], indexToIndex[i + 1] - indexToIndex[i] + 1);
		}
		return output;
	}
	private static List<int> GetIndexesAroundTag(string html, string tag)
	{
		string[] htmlLabels = new string[2]{"<" + tag, "</" + tag};
		List<int> output = new List<int>();
		int maxLength = html.Length - 2;
		for (int i = 0; i < htmlLabels.Length; i++) maxLength -= htmlLabels[i].Length;
		output.Add(0);
		for (int i = 0, j, k, found; i < maxLength; i++)
		{
			//Console.WriteLine(i);
			for (j = 0; j < 2; j++)
			{
				for (k = -1, found = 1; k < htmlLabels[j].Length - 1;)
					if (html[i + ++k] != htmlLabels[j][k]) { found = 0; break; }
				if (found == 1) { output.Add(i - 1); while(html[i + ++k] != '>'); output.Add(i + k + 1); i += k; }
			}
		}
		output.Add(html.Length - 1);
		// for (int i = 0; i + 1 < output.Count(); i++) Console.Write("{0}{1}", output[i], i % 2 == 0 ? ',' : ':');
		return output;
	}
	public static string RemoveTagContent(string html, string tag, string divider = "\n")
	{
		return html.MultiSubstring(GetIndexesAroundTag(html, tag), divider, 4, 0);
	}
	public static string RemoveTag(string html, string tag, string divider = "")
	{
		return html.MultiSubstring(GetIndexesAroundTag(html, tag), divider, 2, 0);
	}
	public static string GetTagContent(string html, string tag, string divider = "\n")
	{
		return html.MultiSubstring(GetIndexesAroundTag(html, tag), divider, 4, 2);
	}
}
/*
public class Program
{
	public static void Main()
	{
		ServicePointManager.Expect100Continue = true;
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		HttpClient client = new HttpClient();
		string url = @"https://en.wikipedia.org/wiki/Pi";
		string html = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
		
		html = html.Replace("\n", "");
		html = HtmlParser.GetTagContent(html, "p", " ");
		html = HtmlParser.RemoveTagContent(html, "semantics", "");
		html = HtmlParser.RemoveTagContent(html, "style", "");
		html = HtmlParser.RemoveTag(html, "");
		
		foreach (char[][] entity in HtmlParser.htmlEntities)
		{
			if(entity[1].Length != 0) html = html.Replace(new string(entity[1]), new string(entity[0]));
			html = html.Replace(new string(entity[2]), new string(entity[0]));
		}
		for (int i = 6, j; i >= 2; i--)
		{
			char[] spaces = new char[i];
			for (j = 0; j < i; j++) spaces[j] = ' ';
			html = html.Replace(new string(spaces), " ");
		}
		
		Console.Write(html);
	}
}
*/