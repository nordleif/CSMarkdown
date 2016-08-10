using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonMark;
using CommonMark.Formatters;
using CommonMark.Syntax;
using HtmlAgilityPack;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSMarkdown.Rendering
{
    public class CSMarkdownRenderer
    {
        public CSMarkdownRenderer()
        {

        }

        public byte[] Render(string text, CSMarkdownRenderOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            if (options == null)
                options = new CSMarkdownRenderOptions();

            // Create context
            var context = new RenderContext { RenderOptions = options };
            
            // Parse YAML and markdown
            var yamlText = string.Empty;
            using (var reader = new StringReader(text))
            {
                using (StringWriter yamlWriter = new StringWriter(), markdownWriter = new StringWriter())
                {
                    var isYamlSection = false;
                    var line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.TrimEnd().Equals("---", StringComparison.InvariantCultureIgnoreCase))
                            isYamlSection = !isYamlSection;
                        else if (isYamlSection)
                            yamlWriter.WriteLine(line);
                        else
                            markdownWriter.WriteLine(line);
                    }
                    yamlText = yamlWriter.ToString();
                    context.MarkdownText = markdownWriter.ToString();
                }
            }

            // Parse YAML header
            context.YamlHeader = YamlOptions.Parse(yamlText);

            // Load HTML document
            context.HtmlDocument = new HtmlDocument();
            context.HtmlDocument.LoadHtmlFromResource("default.html");

            // Render YAML options
            RenderYamlHeader(context);

            // Render markdown text
            RenderCommonMark(context);

            // Parse code chunks
            context.CodeChunks = context.HtmlDocument.DocumentNode.Descendants("code").Where(n => n.Attributes["lang"] != null && n.Attributes["lang"].Value == "cs").Select(n => CodeChunk.Parse(n)).ToArray();

            // Render code chunks
            RenderCodeChunks(context);
            
            // 
            var document = context.HtmlDocument.Flatten();
            var result = Encoding.UTF8.GetBytes(document.DocumentNode.InnerHtml);
            if (options.Output == RenderOutput.Pdf)
            {
                var renderer = new PdfRenderer();
                result = renderer.Render(document.DocumentNode.InnerHtml, new PdfRendererOptions());
            }

            return result;
        }

        private void RenderCodeChunks(RenderContext context)
        {
            if (context.CodeChunks == null || !context.CodeChunks.Any())
                return;

            var scriptContext = new ScriptContext();
            var scriptState = CSharpScript.RunAsync(string.Empty, ScriptOptions.Default, scriptContext, typeof(ScriptContext)).Result;

            foreach (var codeChunk in context.CodeChunks)
            {
                try
                {
                    if (string.IsNullOrEmpty(codeChunk.Code))
                        continue;
                    
                    scriptContext.CurrentNode = HtmlNode.CreateNode("<div>");

                    // eval
                    if (!codeChunk.Options.ReadValue<bool>("eval", true))
                        return;

                    // echo
                    if (codeChunk.Options.ReadValue<bool>("echo", false))
                    {
                        var echoNode = context.HtmlDocument.CreateTextNode($"<pre><code>{codeChunk.Code}</code></pre>");
                        scriptContext.CurrentNode.ChildNodes.Add(echoNode);
                    }

                    // run
                    scriptState = scriptState.ContinueWithAsync(codeChunk.Code).Result;
                }
                catch (CompilationErrorException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (codeChunk.Inline)
                        codeChunk.Node.ParentNode.ReplaceChild(HtmlNode.CreateNode(Convert.ToString(scriptState.ReturnValue)), codeChunk.Node);
                    else
                        codeChunk.Node.ParentNode.ReplaceChild(scriptContext.CurrentNode, codeChunk.Node);
                }

            }
        }
        
        private void RenderCommonMark(RenderContext context)
        {
            if (string.IsNullOrWhiteSpace(context.MarkdownText))
                return;

            var settings = CommonMarkSettings.Default.Clone();
            settings.AdditionalFeatures = CommonMarkAdditionalFeatures.All;
            settings.OutputFormat = OutputFormat.Html;

            // Create syntax tree
            Block document = null;
            using (var reader = new StringReader(context.MarkdownText))
            {
                document = CommonMarkConverter.ProcessStage1(reader, settings);
            }

            // Parse block element into inline elements.
            CommonMarkConverter.ProcessStage2(document, settings);

            //
            var block = document;
            while (block != null)
            {
                switch (block.Tag)
                {
                    case BlockTag.FencedCode:
                        var info = block?.FencedCodeData?.Info?.Trim();
                        if (!string.IsNullOrWhiteSpace(info) && info.StartsWith("{s", StringComparison.InvariantCultureIgnoreCase) && info.EndsWith("}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var html = $"<code lang=\"cs\" options=\"{info}\">{block.StringContent.ToBase64()}</code>";
                            block.Tag = BlockTag.HtmlBlock;
                            block.StringContent = new StringContent();
                            block.StringContent.Append(html, 0, html.Length);
                        }
                        break;
                }

                if (block.InlineContent != null && !string.IsNullOrEmpty(block.InlineContent.LiteralContent))
                {
                    var text = block.InlineContent.LiteralContent;
                    var index = text.IndexOf("'s");
                    if (index >= 0)
                    {
                        var nextSibling = block.InlineContent.NextSibling;

                        var prefix = string.Empty;
                        var code = string.Empty;
                        var suffix = string.Empty;

                        prefix = text.Substring(0, index);
                        text = text.Substring(index + 2);
                        index = text.IndexOf("'");
                        code = text.Substring(0, index).Trim().ToBase64();
                        suffix = text.Substring(index + 1);

                        block.InlineContent = new Inline
                        {
                            Tag = InlineTag.String,
                            LiteralContent = prefix,
                            NextSibling = new Inline
                            {
                                Tag = InlineTag.RawHtml,
                                LiteralContent = $"<code lang=\"cs\" options=\"inline\">{code}</code>",
                                NextSibling = new Inline
                                {
                                    Tag = InlineTag.String,
                                    LiteralContent = suffix,
                                    NextSibling = nextSibling
                                }
                            }
                        };
                    }
                }

                if (block.FirstChild != null)
                    block = block.FirstChild;
                else if (block.NextSibling != null)
                    block = block.NextSibling;
                else
                    block = null;
            }

            // Convert document to HTML
            var htmlText = string.Empty;
            using (var writer = new StringWriter())
            {
                CommonMarkConverter.ProcessStage3(document, writer, settings);
                htmlText = writer.ToString();
            }

            //
            var mainNode = context.HtmlDocument.DocumentNode.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("main-container"));
            if (mainNode != null)
                mainNode.InnerHtml = $"{mainNode.InnerHtml}\r\n{htmlText}";
        }

        private void RenderYamlHeader(RenderContext context)
        {
            var styleUrls = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            styleUrls.Add("default", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css");
            styleUrls.Add("cerulean", "https://bootswatch.com/cerulean/bootstrap.min.css");
            styleUrls.Add("cosmo", "https://bootswatch.com/cosmo/bootstrap.min.css");
            styleUrls.Add("cyborg", "https://bootswatch.com/cyborg/bootstrap.min.css");
            styleUrls.Add("darkly", "https://bootswatch.com/darkly/bootstrap.min.css");
            styleUrls.Add("flatly", "https://bootswatch.com/flatly/bootstrap.min.css");
            styleUrls.Add("journal", "https://bootswatch.com/journal/bootstrap.min.css");
            styleUrls.Add("lumen", "https://bootswatch.com/lumen/bootstrap.min.css");
            styleUrls.Add("paper", "https://bootswatch.com/paper/bootstrap.min.css");
            styleUrls.Add("simplex", "https://bootswatch.com/simplex/bootstrap.min.css");
            styleUrls.Add("slate", "https://bootswatch.com/slate/bootstrap.min.css");
            styleUrls.Add("spacelab", "https://bootswatch.com/spacelab/bootstrap.min.css");
            styleUrls.Add("superhero", "https://bootswatch.com/superhero/bootstrap.min.css");
            styleUrls.Add("united", "https://bootswatch.com/united/bootstrap.min.css");
            styleUrls.Add("yeti", "https://bootswatch.com/yeti/bootstrap.min.css");
            
            // Theme
            var theme = context.YamlHeader.ReadValue<string>("output.html_document.theme", "default");
            if (!theme.Equals("default", StringComparison.InvariantCultureIgnoreCase) && styleUrls.ContainsKey(theme))
            {
                var styleNode = context.HtmlDocument.DocumentNode.Descendants("link").FirstOrDefault();
                if (styleNode != null)
                    styleNode.Attributes["href"].Value = styleUrls[theme];
            }

            // Title
            var title = context.YamlHeader.ReadValue<string>("title", string.Empty);
            if (!string.IsNullOrWhiteSpace(title))
            {
                var titleNode = context.HtmlDocument.DocumentNode.Descendants("title").FirstOrDefault();
                if (titleNode != null)
                    titleNode.InnerHtml = title;
            }

            // main-container
            var mainNode = context.HtmlDocument.DocumentNode.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("main-container"));
            if (mainNode != null)
            {
                if (!string.IsNullOrWhiteSpace(title))
                    mainNode.InnerHtml = $"{mainNode.InnerHtml}\r\n<div><h1>{title}</h1></div>";
            }
        }
    }
}