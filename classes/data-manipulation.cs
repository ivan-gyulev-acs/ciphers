using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

// Number Information And Manipulation
static class NIAM
{
	public static int GCD(int a, int b)
	{
		if (a == 0 || b == 0) throw new Exception("Division by 0!");
		if (a % b == 0) return b;
		return GCD(b, a % b);
	}
}

// Text Information And Manipulation
static class TIAM
{
	public const string EnglishWordsFilePath = @"..\data\english-words.txt";
	public static Dictionary<int, List<char[]>> EnglishWordsByLength = new Dictionary<int, List<char[]>>();
	static TIAM()
	{
		char[][] unsortedWords = File.ReadAllLines(EnglishWordsFilePath).Select(word => word.ToArray()).ToArray();
		foreach (char[] word in unsortedWords)
		{
			if(!EnglishWordsByLength.ContainsKey(word.Length))
				EnglishWordsByLength.Add(word.Length, new List<char[]>());
			EnglishWordsByLength[word.Length].Add(word);
		}
	}
	public static bool Contains(this char[][] input, char[] search)
	{
		int start = 0, end = input.Length - 1, currentArray = (start + end) / 2;
		while (end >= start && start >= 0 && end <= input.Length - 1)
		{
			for (int i = 0; i < search.Length; i++)
			{
				if (i == input[currentArray].Length || search[i] > input[currentArray][i])
				{
					start = currentArray + 1;
					currentArray = (start + end) / 2;
					break;
				}
				if (i == search.Length - 1 && i < input[currentArray].Length - 1 || search[i] < input[currentArray][i])
				{
					end = currentArray - 1;
					currentArray = (start + end) / 2;
					break;
				}
				else if (i == search.Length - 1 && search.Length == input[currentArray].Length) return true;
			}
		}
		return false;
	}
	public static bool IsEnglishWord(this char[] word)
	{
		return EnglishWordsByLength[word.Length].Contains(word);
	}
	public static bool IsEnglishText(this char[] text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] >= 'A' && text[i] <= 'Z') text[i] = (char)(text[i] + 'a' - 'A');
			else if (text[i] < 'a' || text[i] > 'z') text[i] = ' ';
		}
		char[][] words = text.Split(' ');
		int wordLengthSum = 0;
		foreach (char[] word in words) if (EnglishWordsByLength[word.Length].Contains(word)) wordLengthSum += word.Length;
		return wordLengthSum * 100 >= text.Length * 60;
	}
	public static int Find(this char[] input, char[] search)
	{
		bool characterMathing;
		for (int i = 0; i < input.Length - search.Length + 1; i++)
		{
			characterMathing = true;
			for (int j = 0; j < search.Length; j++)
			{
				if (input[i + j] != search[j])
				{
					characterMathing = false;
					break;
				}
			}
			if (characterMathing) return i;
		}

		return -1;
	}
	public static char[] Get(this char[] input, int start, int end)
	{
		if (start > end)
		{
			start ^= end;
			end   ^= start;
			start ^= end;
		}

		if (start < 0 || end > input.Length - 1) throw new Exception("Out Of Range!");

		char[] output = new char[end - start + 1];
		for(int a = 0; a < output.Length; a++)
		{
			output[a] = input[start + a];
		}

		return output;
	}
	public static char[] Wrap(this char[] input, int lineLength)
	{
		if (input == null)  throw new Exception("Null Pointer Exception!");
		if (lineLength < 0) throw new Exception("Invalid lineLenght: " + lineLength);

		char[] output;
		if (lineLength > input.Length)
		{
			output = new char[input.Length + 1];
			output[0] = '\t';
			for(int i = 0; i < input.Length; i++)
				output[i + 1] = input[i];
			return output;
		}

		output = new char[input.Length + input.Length / lineLength * 2 + 1];
		for (int i = 0; i < input.Length / lineLength + 1; i++)
		{
			output[i * (lineLength + 2)] = '\t';
			for (int j = 0; j < lineLength - (i + 1) * lineLength % input.Length % lineLength; j++)
				output[i * (lineLength + 2) + 1 + j] = input[i * lineLength + j];
		}
		for (int i = 0; i < input.Length / lineLength; i++)
			output[(i + 1) * (lineLength + 2) - 1] = '\n';
		return output;
	}
	public static char[][] Split(this char[] input, char splitter)
	{
		List<char[]> output = new List<char[]>();
		for (int i = 0, start; i < input.Length - 1; )
		{
			while (i + 1 < input.Length && input[i] == splitter) i++;
			start = i;
			while (i < input.Length && input[i] != splitter) i++;
			output.Add(new char[i - start]);
			for (int j = start; j < i; j++) output.Last()[j - start] = input[j];
		}
		return output.ToArray();
	}
}