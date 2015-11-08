using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CasualMeter.Common.Formatters
{
    public class Formatter
    {
        protected Dictionary<string, object> Placeholders;
        protected IFormatProvider FormatProvider;

        public Formatter() { }

        public Formatter(IEnumerable<KeyValuePair<string, object>> placeholders, IFormatProvider formatProvider)
        {
            Placeholders = placeholders.ToDictionary(x => x.Key, x => x.Value);
            FormatProvider = formatProvider;
        }

        private string ReplacePlaceHolder(string[] parts)
        {
            var key = parts[0];
            string format = null;
            if (parts.Length > 2)
                throw new FormatException("Too many parts in a place holder");
            if (parts.Length == 2)
                format = parts[1];
            object value;
            if (!Placeholders.TryGetValue(key, out value))
                throw new FormatException(string.Format("Unknown placeholder '{0}'", key));
            return ToString(value, format, FormatProvider);
        }

        private static string ToString(object o, string format, IFormatProvider formatProvider)
        {
            var formattable = o as IFormattable;
            if (formattable != null)
                return formattable.ToString(format, formatProvider);
            return o.ToString();
        }

        public string Replace(string input)
        {
            return Replace(input, ReplacePlaceHolder);
        }

        private static string Replace(string input, Func<string[], string> placeHolderFunc)
        {
            bool isEscape = false;
            bool isPlaceHolder = false;
            var result = new StringBuilder();
            var placeHolderPart = new StringBuilder();
            var placeHolderParts = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (!isEscape)
                {
                    switch (c)
                    {
                        case '{':
                            if (isPlaceHolder)
                                throw new FormatException("Unxpected '{'");
                            isPlaceHolder = true;
                            goto nextChar;
                        case '}':
                            if (!isPlaceHolder)
                                throw new FormatException("Unxpected '}'");
                            placeHolderParts.Add(placeHolderPart.ToString());
                            result.Append(placeHolderFunc(placeHolderParts.ToArray()));
                            placeHolderPart.Clear();
                            placeHolderParts.Clear();
                            isPlaceHolder = false;
                            goto nextChar;
                        case '\\':
                            isEscape = true;
                            goto nextChar;
                        case ':':
                            if (!isPlaceHolder)
                                goto printChar;
                            placeHolderParts.Add(placeHolderPart.ToString());
                            placeHolderPart.Clear();
                            goto nextChar;
                    }
                }
            printChar:
                if (isPlaceHolder)
                    placeHolderPart.Append(c);
                else
                    result.Append(c);
                nextChar:;
            }
            if (isEscape)
                throw new FormatException("End of string while in escape mode");
            if (isPlaceHolder)
                throw new FormatException("End of string while in placeholder mode");
            return result.ToString();
        }

    }
}
