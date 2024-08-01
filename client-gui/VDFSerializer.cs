using System.Text;

namespace ACCConnector {
    public class VDFSerializer {
        public static Tuple<string, object> Deserialize(TextReader reader) {
            var rootKey = QuotedString(reader);
            var rootValue = Value(reader);
            return new Tuple<string, object>(rootKey, rootValue);
        }

        private static string QuotedString(TextReader reader) {
            Consume('"', reader);
            var sb = new StringBuilder();
            while (reader.Peek() != '\"') {
                sb.Append(PossiblyQuotedChar(reader));
            }
            Consume('"', reader);
            return sb.ToString();
        }

        private static char PossiblyQuotedChar(TextReader reader) {
            int c = reader.Read();
            if (c == -1) {
                throw new InvalidVDFException("Unexpected EOF");
            }
            if (c == '\\') {
                c = reader.Read();
                if (c == -1) {
                    throw new InvalidVDFException("Unexpected EOF");
                }
            }
            return (char)c;
        }

        private static Dictionary<string, object> KVDict(TextReader reader) {
            var dict = new Dictionary<string, object>();
            Consume('{', reader);
            SkipWhitespace(reader);
            while (reader.Peek() != '}') {
                SkipWhitespace(reader);
                var key = QuotedString(reader);
                var value = Value(reader);
                dict.Add(key, value);
                SkipWhitespace(reader);
            }
            Consume('}', reader);
            return dict;
        }

        private static object Value(TextReader reader) {
            SkipWhitespace(reader);
            int c = reader.Peek();
            return c switch {
                -1 => throw new InvalidVDFException("Unexpected EOF"),
                '"' => QuotedString(reader),
                '{' => KVDict(reader),
                _ => throw new InvalidVDFException($"Unexpected character {(char)c}"),
            };
        }

        private static void SkipWhitespace(TextReader reader) {
            while (true) {
                int c = reader.Peek();
                if (c != -1 && Char.IsWhiteSpace((char)c)) {
                    reader.Read();
                } else {
                    break;
                }
            }
        }

        private static void Consume(char c, TextReader reader) {
            int r = reader.Read();
            if (r == -1) {
                throw new InvalidVDFException("Unexpected EOF");
            }
            if (r != c) {
                throw new InvalidVDFException($"Expected {c} got {(char)r}");
            }
        }
    }

    public class InvalidVDFException : Exception {
        public InvalidVDFException(string? message) : base(message) {
        }
    }
}
