using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionBlocks
{
    /// <summary>
    ///     Lines of text whose parts (shell items, property stores, strings) are only turned into text when ToString() is
    ///     called. A part that can't be shown says so instead of throwing
    /// </summary>
    internal class LazyText
    {
        private readonly List<object> _lines = new List<object>();

        public void AppendLine(object line)
        {
            _lines.Add(line);
        }

        /// <summary>A part made only when the text is (a shell item parsed then: most callers never look at it)</summary>
        public void AppendLine(Func<object> line)
        {
            _lines.Add(line);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var line in _lines)
            {
                string text;
                try
                {
                    text = (line is Func<object> make ? make() : line)?.ToString();
                }
                catch (Exception ex)
                {
                    text = $"(could not be shown: {ex.Message})";
                }

                sb.AppendLine(text);
            }

            return sb.ToString();
        }
    }
}