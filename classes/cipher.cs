using System;
using System.Collections.Generic;

public static class Cipher
{
    public const int StartChar = ' ';
    public const int EndChar   = '~';
    public const int AlphabetLength = EndChar - StartChar + 1;
    private static Random random = new Random();
    public static class Caesar
    {
        public static bool IsValidKey(int key)
        {
            if (key < 0) return false;
            return true;
        }
        public static int GetRandomKey()
        {
            return random.Next(1, AlphabetLength);
        }
        public static char[] Encode(char[] input, int key)
        {
            if (!IsValidKey(key)) throw new ArgumentException("Invalid Key | Cipher.Caesar.Encode(char[] input, int key)");

            key %= AlphabetLength;
            char[] output = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
                output[i] = (char)((input[i] - StartChar + AlphabetLength + key) % AlphabetLength + StartChar);

            return output;
        }
        public static char[] Decode(char[] input, int key)
        {
            if (!IsValidKey(key)) throw new ArgumentException("Invalid Key | Cipher.Caesar.Encode(char[] input, int key)");

            key %= AlphabetLength;
            char[] output = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
                output[i] = (char)((input[i] - StartChar + AlphabetLength - key) % AlphabetLength + StartChar);

            return output;
        }
        public static List<char[]> BruteForce(char[] input)
        {
            List<char[]> output = new List<char[]>();
            char[] decodedMessage;
            for (int a = 0; a < AlphabetLength; a++)
            {
                decodedMessage = Decode(input, a);
                if (decodedMessage.IsEnglishText()) output.Add(decodedMessage);
            }
            return output;
        }
        public static bool Check()
        {
            char[] text = TextGenerator.GetText();
            int key = GetRandomKey();
            char[] output = Decode(Encode(text, key), key);
            for (int a = 0; a < text.Length; a++) if (text[a] != output[a]) return false;
            return true;
        }
    }
    public static class Transposition
    {
        public static bool IsValidKey(int key, char[] input)
        {
            if(key < input.Length / 10 || key > input.Length / 3 + 1) return false;
            return true;
        }
        public static int GetRandomKey(char[] input)
        {
            return random.Next(input.Length / 10, input.Length / 3 + 1);
        }
        public static char[] Encode(char[] input, int key)
        {
            if (!IsValidKey(key, input)) throw new ArgumentException("Invalid Key | Cipher.Transposition.Encode(char[] input, int key)");

            int error = (key - input.Length % key) % key;
            int rows = input.Length / key + (error == 0 ? 0 : 1);
            int columns = key;
            char[] output = new char[input.Length];
            int offset = 0;

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (j * columns + i < input.Length) output[i * rows + j - offset] = input[j * columns + i];
                    else offset++;
                }
            }

            return output;
        }
        public static char[] Decode(char[] input, int key)
        {
            if (!IsValidKey(key, input)) throw new ArgumentException("Invalid Key | Cipher.Transposition.Decode(char[] input, int key)");

            int error = (key - input.Length % key) % key;
            int rows = input.Length / key + (error == 0 ? 0 : 1);
            int columns = key;
            char[] output = new char[input.Length];
            int offset = 0;
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (j * columns + i < input.Length) output[j * columns + i] = input[i * rows + j - offset];
                    else offset++;
                }
            }

            return output;
        }
        public static List<char[]> BruteForce(char[] input)
        {
            List<char[]> output = new List<char[]>();
            char[] decodedMessage;
            int start = input.Length / 10;
            int end = input.Length / 3;
            for (int i = start; i <= end; i++)
            {
                decodedMessage = Decode(input, i);
                if (decodedMessage.IsEnglishText()) output.Add(decodedMessage);
            }
            return output;
        }
        public static bool Check()
        {
            char[] text = TextGenerator.GetText();
            int key = GetRandomKey(text);
            char[] output = Decode(Encode(text, key), key);
            for (int i = 0; i < text.Length; i++) if (text[i] != output[i]) return false;
            return true;
        }
    }   
    public static class Multiplicative
    {
        private static int DecodingAlgorigthm(int key, int letter, int offset = 0)
        {
            if (letter % key == offset) return letter / key;
            else return (AlphabetLength - offset) / key + 1 + DecodingAlgorigthm(key, letter,
                 (((AlphabetLength - offset) / key + 1) * key + offset) % AlphabetLength % key);
        }
        public static bool IsValidKey(int key)
        {
            if (key <= 0 && NIAM.GCD(AlphabetLength, key) != 1) return false;
            return true;
        }
        public static int GetRandomKey()
        {
            int key;
            do key = random.Next(2, AlphabetLength);
            while (NIAM.GCD(AlphabetLength, key) != 1);
            return key;
        }
        public static char[] Encode(char[] input, int key)
        {
            Console.WriteLine("For alphabet length {0}, key {1} is {2}.", AlphabetLength, key, (IsValidKey(key) ? "valid" : "invalid"));
            if (!IsValidKey(key)) throw new ArgumentException("Invalid Key | Cipher.Multiplicative.Encode(char[] input, int key)");

            char[] output = new char[input.Length];
            for (int i = 0; i < input.Length; i++) output[i] = (char)(StartChar + (input[i] - StartChar) * key % AlphabetLength);
            return output;
        }
        public static char[] Decode(char[] input, int key)
        {
            Console.WriteLine("For alphabet length {0}, key {1} is {2}.", AlphabetLength, key, (IsValidKey(key) ? "valid" : "invalid"));
            if (!IsValidKey(key)) throw new ArgumentException("Invalid Key | Cipher.Multiplicative.Decode(char[] input, int key)");

            char[] output = new char[input.Length];
            for (int i = 0; i < input.Length; i++) output[i] = (char)(StartChar + DecodingAlgorigthm(key, input[i] - StartChar));
            return output;
        }
        public static List<char[]> BruteForce(char[] input)
        {
            List<char[]> output = new List<char[]>();
            char[] decodedMessage;
            for (int i = 1; i < AlphabetLength; i++)
            {
                if (!IsValidKey(i)) continue;
                decodedMessage = Decode(input, i);
                if (decodedMessage.IsEnglishText()) output.Add(decodedMessage);
            }
            return output;
        }
        public static bool Check()
        {
            char[] text = TextGenerator.GetText();
            int key = GetRandomKey();
            char[] output = Decode(Encode(text, key), key);
            for (int i = 0; i < text.Length; i++) if (text[i] != output[i]) return false;
            return true;
        }
    }
    public static class Substitution
    {
        public static bool IsValidKey(char[] key)
        {
            if (key.Length != AlphabetLength) return false;
            bool[] symbolPresence = new bool[AlphabetLength];
            for (int i = 0; i < symbolPresence.Length; i++) symbolPresence[i] = false;
            foreach (char symbol in key)
            {
                if (symbol < StartChar || symbol > EndChar) return false;
                else symbolPresence[symbol - StartChar] = true;
            }
            foreach (bool isPresent in symbolPresence) if (!isPresent) return false;
            return true;
        }
        public static char[] GetRandomKey()
        {
            char[] key = new char[AlphabetLength];
            for (int i = 0; i < AlphabetLength; i++) key[i] = (char)(StartChar + i);
            for (int i = 0, swapA, swapB; i < AlphabetLength * 6; i++)
            {
                swapA = random.Next(AlphabetLength);
                swapB = (swapA + random.Next(1, AlphabetLength)) % AlphabetLength;
                key[swapA] = (char)(key[swapA] ^ key[swapB]);
                key[swapB] = (char)(key[swapB] ^ key[swapA]);
                key[swapA] = (char)(key[swapA] ^ key[swapB]);
            }
            return key;
        }
        public static char[] Encode(char[] input, char[] key)
        {
            if (!IsValidKey(key)) throw new ArgumentException("Invalid Key | Cipher.Substitution.Encode(char[] input, char[] key)");

            char[] output = new char[input.Length];
            for (int i = 0; i < input.Length; i++) output[i] = key[input[i] - StartChar];

            return output;
        }
        public static char[] Decode(char[] input, char[] key)
        {
            if (!IsValidKey(key)) throw new ArgumentException("Invalid Key | Cipher.Substitution.Decode(char[] input, char[] key)");

            char[] output = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (input[i] == key[j])
                    {
                        output[i] = (char)(StartChar + j);
                        break;
                    }
                }
            }

            return output;
        }
        public static char[][] BruteForce(char[] input)
        {
            char[][] output = new char[1][];
            output[0] = input;
            return output;
        }
        public static bool Check()
        {
            char[] text = TextGenerator.GetText();
            char[] key = GetRandomKey();
            char[] output = Decode(Encode(text, key), key);
            for (int a = 0; a < text.Length; a++) if (text[a] != output[a]) return false;
            return true;
        }
    }
}