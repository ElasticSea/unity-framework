using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Core.Extensions
{
	public static class StringExtensions
	{
        /// <summary>
        /// Eg MY_INT_VALUE => MyIntValue
        /// </summary>
        public static string ToTitleCase(this string input)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i];
                if (current == '_' && i + 1 < input.Length)
                {
                    var next = input[i + 1];
                    if (char.IsLower(next))
                        next = char.ToUpper(next);
                    builder.Append(next);
                    i++;
                }
                else
                    builder.Append(current);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Performs a simple char-by-char comparison to see if input ends with postfix
        /// </summary>
        /// <returns></returns>
        public static bool IsPostfix(this string input, string postfix)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (postfix == null)
                throw new ArgumentNullException("postfix");

            if (input.Length < postfix.Length)
                return false;

            for (int i = input.Length - 1, j = postfix.Length - 1; j >= 0; i--, j--)
                if (input[i] != postfix[j])
                    return false;
            return true;
        }

        /// <summary>
        /// Performs a simple char-by-char comparison to see if input starts with prefix
        /// </summary>
        /// <returns></returns>
        public static bool IsPrefix(this string input, string prefix)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (prefix == null)
                throw new ArgumentNullException("prefix");

            if (input.Length < prefix.Length)
                return false;

            for (int i = 0; i < prefix.Length; i++)
                if (input[i] != prefix[i])
                    return false;
            return true;
        }

		public static Enum ToEnum(this string str, Type enumType)
		{
			return Enum.Parse(enumType, str) as Enum;
		}

		public static T ToEnum<T>(this string str)
		{
			return (T)Enum.Parse(typeof(T), str);
		}

		public static string FormatWith(this string str, params object[] args)
		{
			return string.Format(str, args);
		}

		/// <summary>
		/// Parses the specified string to the enum value of type T
		/// </summary>
		public static T ParseEnum<T>(this string value)
		{
			return (T)Enum.Parse(typeof(T), value, false);
		}

		/// <summary>
		/// Parses the specified string to the enum whose type is specified by enumType
		/// </summary>
		public static Enum ParseEnum(this string value, Type enumType)
		{
			return (Enum)Enum.Parse(enumType, value, false);
		}

		/// <summary>
		/// Returns the Nth index of the specified character in this string
		/// </summary>
		public static int IndexOfNth(this string str, char c, int n)
		{
			int s = -1;

			for (int i = 0; i < n; i++)
			{
				s = str.IndexOf(c, s + 1);
				if (s == -1) break;
			}
			return s;
		}

		/// <summary>
		/// Removes the last occurance of the specified string from this string.
		/// Returns the modified version.
		/// </summary>
		public static string RemoveLastOccurance(this string s, string what)
		{
			return s.Substring(0, s.LastIndexOf(what));
		}

		/// <summary>
		/// Removes the type extension. ex "Medusa.mp3" => "Medusa"
		/// </summary>
		public static string RemoveExtension(this string s)
		{
			return s.Substring(0, s.LastIndexOf('.'));
		}

		/// <summary>
		/// Returns whether or not the specified string is contained with this string
		/// Credits to JaredPar http://stackoverflow.com/questions/444798/case-insensitive-containsstring/444818#444818
		/// </summary>
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf(toCheck, comp) >= 0;
		}

		/// <summary>
		/// "tHiS is a sTring TesT" -> "This Is A String Test"
		/// Credits: http://extensionmethod.net/csharp/string/topropercase 
		/// </summary>
		public static string ToProperCase(this string text)
		{
			CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
			TextInfo textInfo = cultureInfo.TextInfo;
			return textInfo.ToTitleCase(text);
		}

		/// <summary>
		/// Ex: "thisIsCamelCase" -> "this Is Camel Case"
		/// Credits: http://stackoverflow.com/questions/155303/net-how-can-you-split-a-caps-delimited-string-into-an-array
		/// </summary>
		public static string SplitCamelCase(this string input)
		{
			return Regex.Replace(input, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
		}

		/// <summary>
		/// Ex: "thisIsCamelCase" -> "This Is Camel Case"
		/// </summary>
		public static string SplitPascalCase(this string input)
		{
			return string.IsNullOrEmpty(input) ? input : input.SplitCamelCase().ToUpperAt(0);
		}

		/// <summary>
		/// Normalizes this string by replacing all 'from's by 'to's and returns the normalized instance
		/// Ex: "path/to\dir".NormalizePath('/', '\\') => "path\\to\\dir"
		/// </summary>
		public static string NormalizePath(this string input, char from, char to)
		{
			return input.Replace(from, to);
		}

		/// <summary>
		/// Replaces the character specified by the passed index with newChar and returns the new string instance
		/// </summary>
		public static string ReplaceAt(this string input, int index, char newChar)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			var builder = new StringBuilder(input);
			builder[index] = newChar;
			return builder.ToString();
		}

		/// <summary>
		/// Uppers the character specified by the passed index and returns the new string instance
		/// </summary>
		public static string ToUpperAt(this string input, int index)
		{
			return input.ReplaceAt(index, char.ToUpper(input[index]));
		}

		/// <summary>
		/// Returns true if this string is null or empty
		/// </summary>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

	    public static string ReplaceAllWhiteSpace(this string str, string replaceWith)
	    {
	        return Regex.Replace(str, @"\s+", replaceWith);
        }

	    public static IEnumerable<IEnumerable<T>> SplitToChunks<T>(this List<T> str, int maxChunkSize)
	    {
	        for (int i = 0; i < str.Count; i += maxChunkSize)
	            yield return str.GetRange(i, Math.Min(maxChunkSize, str.Count - i));
	    }
	     /// <summary>
    /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
    /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
    /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
    /// </summary>
    public static bool IsSubPathOf(this string path, string baseDirPath)
    {
        var normalizedPath = NormalizePath(path);
        var normalizedBaseDirPath = NormalizePath(baseDirPath);

        if (normalizedPath == normalizedBaseDirPath)
	        return false;
        
        return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
    }
	     
     public static string NormalizePath(this string path)
     {
	     var normalizeSlashes = path.Replace('\\', '/');
	     
	     // if has trailing slash then it's a directory
	     if (normalizeSlashes.EndsWith("/"))
	     {
		     return Path.GetFullPath(normalizeSlashes.WithEnding("/"));
	     }

	     return Path.GetFullPath(normalizeSlashes);
     }

    /// <summary>
    /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
    /// results in satisfying .EndsWith(ending).
    /// </summary>
    /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
    public static string WithEnding(this string str, string ending)
    {
        if (str == null)
            return ending;

        string result = str;

        // Right() is 1-indexed, so include these cases
        // * Append no characters
        // * Append up to N characters, where N is ending length
        for (int i = 0; i <= ending.Length; i++)
        {
            string tmp = result + ending.Right(i);
            if (tmp.EndsWith(ending))
                return tmp;
        }

        return result;
    }

    /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
    /// <param name="value">The string to retrieve the substring from.</param>
    /// <param name="length">The number of characters to retrieve.</param>
    /// <returns>The substring.</returns>
    public static string Right(this string value, int length)
    {
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
        }

        return (length < value.Length) ? value.Substring(value.Length - length) : value;
    }
    
		public static string Truncate(this string value, int maxChars)
		{
			return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
		}
		public static string TruncateLast(this string value, int maxChars)
		{
			return value.Length <= maxChars ? value : "..." + value.Substring(value.Length-maxChars, maxChars);
		}
		
		public static bool InvariantIgnore(this string str, string other)
		{
			return string.Equals(str, other, StringComparison.InvariantCultureIgnoreCase);
		}
    }
}
