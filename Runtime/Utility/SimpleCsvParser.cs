using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Noo.Tools
{
    public static class SimpleCsvParser
    {
        public static IEnumerable<IList<string>> Parse(string content, char delimiter, char qualifier)
        {
            using (var reader = new StringReader(content))
            {
                return Parse(reader, delimiter, qualifier).ToArray();
            }
        }

        public static IEnumerable<IList<string>> Parse(TextReader reader, char delimiter, char qualifier)
        {
            var inQuote = false;
            var record = new List<string>();
            var sb = new StringBuilder();

            while (reader.Peek() != -1)
            {
                var readChar = (char)reader.Read();

                if (readChar == '\n' || (readChar == '\r' && (char)reader.Peek() == '\n'))
                {
                    if (readChar == '\r')
                    {
                        reader.Read();
                    }

                    if (inQuote)
                    {
                        if (readChar == '\r')
                        {
                            sb.Append('\r');
                        }

                        sb.Append('\n');
                    }
                    else
                    {
                        if (record.Count > 0 || sb.Length > 0)
                        {
                            record.Add(sb.ToString());
                            sb.Clear();
                        }

                        if (record.Count > 0)
                        {
                            yield return record;
                        }

                        record = new List<string>(record.Count);
                    }
                }
                else if (sb.Length == 0 && !inQuote)
                {
                    if (readChar == qualifier)
                    {
                        inQuote = true;
                    }
                    else if (readChar == delimiter)
                    {
                        record.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (!char.IsWhiteSpace(readChar))
                    {
                        sb.Append(readChar);
                    }
                        
                }
                else if (readChar == delimiter)
                {
                    if (inQuote)
                    {
                        sb.Append(delimiter);
                    }
                    else
                    {
                        record.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else if (readChar == qualifier)
                {
                    if (inQuote)
                    {
                        if ((char)reader.Peek() == qualifier)
                        {
                            reader.Read();
                            sb.Append(qualifier);
                        }
                        else
                        {
                            inQuote = false;
                        }
                    }
                    else
                    {
                        sb.Append(readChar);
                    }
                }
                else
                {
                    sb.Append(readChar);
                }
            }

            if (record.Count > 0 || sb.Length > 0)
            {
                record.Add(sb.ToString());
            }

            if (record.Count > 0)
            {
                yield return record;
            }
        }
    }
}

