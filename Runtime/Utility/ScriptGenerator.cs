using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Noo.Tools
{
    public class ScriptGenerator
    {
        public readonly struct IndentLevel : IDisposable
        {
            readonly ScriptGenerator generator;

            public IndentLevel(ScriptGenerator generator)
            {
                this.generator = generator;
                generator.indent++;
            }

            public void Dispose() => generator.indent--;
        }

        public readonly struct BracesLevel : IDisposable
        {
            readonly ScriptGenerator generator;

            public BracesLevel(ScriptGenerator generator)
            {
                this.generator = generator;
                generator.Line("{");
                generator.indent++;
            }

            public void Dispose()
            {
                generator.indent--;
                generator.Line("}");
            }
        }

        public readonly struct ConditionScope : IDisposable
        {
            readonly ScriptGenerator generator;
            readonly string condition;

            public ConditionScope(ScriptGenerator generator, string condition)
            {
                this.condition = condition;
                this.generator = generator;
                generator.Pragma($"if {condition}");
            }

            public void Dispose()
            {
                generator.Pragma($"endif // {condition}");
            }
        }

        public readonly struct RegionLevel : IDisposable
        {
            readonly ScriptGenerator generator;
            readonly string region;

            public RegionLevel(ScriptGenerator generator, string region)
            {
                this.generator = generator;
                this.region = region;
                generator.Space(); 
                generator.Pragma($"region {region}");
                generator.Space();
            }

            public void Dispose()
            {
                generator.Space();
                generator.Pragma($"endregion // {region}");
                generator.Space();
            }
        }

        public readonly struct CommentScope : IDisposable
        {
            readonly ScriptGenerator generator;

            public CommentScope(ScriptGenerator generator, string blockName)
            {
                this.generator = generator;
                generator.Line($"// ****** {blockName} ******");
            }

            public void Dispose()
            {
                //generator.Line($"// ****** ");
                generator.Space();
            }
        }

        public readonly struct FileScope : IDisposable
        {
            readonly ScriptGenerator generator;
            readonly string fileName;

            public FileScope(ScriptGenerator generator, string fileName)
            {
                this.generator = generator;
                this.fileName = fileName;
            }

            public void Dispose()
            {
                generator.SaveAndClear(fileName);
            }
        }

        readonly StringBuilder sb = new();
        readonly string outputPath;
        int indent;
        public bool lastLineWasBrace;

        public ScriptGenerator(string outputPath)
        {
            this.outputPath = outputPath;
        }

        public void Line(string text)
        {
            var isBrace = text.Trim() == "}";
            var isMetaLine = isBrace || text.StartsWith("#");
            if (lastLineWasBrace && !isMetaLine && sb.Length > 0) Space();
            lastLineWasBrace = isBrace;
            sb.AppendLine($"{new string(' ', indent * 4)}{text}");
        }

        public void LineIf(bool condition, string text)
        {
            if (condition) Line(text);
        }

        public void LineIf(bool condition, string text, string textElse)
        {
            if (condition) Line(text); else Line(textElse);
        }

        public void Write(string text)
        {
            sb.Append(text);
        }

        public void Space() => sb.AppendLine("");

        public IndentLevel Indent() => new(this);
        public BracesLevel Section() => new(this);

        public BracesLevel Section(string title)
        {
            Line(title);
            return new(this);
        }

        public ConditionScope Conditional(string condition)
        {
            return new ConditionScope(this, condition);
        }

        public void Pragma(string pragma)
        {
            Line($"#{pragma}");
        }

        public RegionLevel Region(string region)
        {
            return new RegionLevel(this, region);
        }

        public string Choose(bool condition, string a, string b = "") => condition ? a : b;

        public void Comment(string comment) => Line($"// {comment}");
        public void Summary(string comment) => LineIf(!string.IsNullOrWhiteSpace(comment), $"/// <summary>{comment}</summary>");

        public CommentScope CommentBlock(string region)
        {
            return new CommentScope(this, region);
        }

        public void SaveAndClear(string fileName)
        {
            Directory.CreateDirectory(outputPath);
            fileName = Path.Combine(outputPath, $"{fileName}");
            File.WriteAllText(fileName, sb.ToString());
            sb.Clear();
        }

        public FileScope WriteFile(string fileName)
        {
            return new FileScope(this, fileName);
        }

        public string VarName(string s)
        {
            return string.Join("", s.AsEnumerable().Where(chr => char.IsLetter(chr) || char.IsDigit(chr)));
        }

        public override string ToString() => sb.ToString();
    }
}
