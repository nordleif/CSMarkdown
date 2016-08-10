using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CSMarkdown.Rendering;

namespace CSMarkdown.Tests
{
    [TestFixture]
    public class YamlOptionsTests
    {
        [TestCase("title: \"Title\"\r\noutput:\r\n  html_document:\r\n    toc: true\r\n    theme: united ", "output.html_document.theme", "united")]
        [TestCase("title: \"Title\"\r\noutput:\r\n  html_document:\r\n    toc: true\r\n    theme: united ", "OUTPUT.HTML_DOCUMENT.THEME", "united")]
        [TestCase("title: \"Title\"\r\noutput:", "", "")]
        [TestCase("nunc est bibendum", "", "")]
        [TestCase("", "", "")]
        [Test]
        public void YamlOptions_Parse(string yamlText, string expectedKey, string expectedValue)
        {
            var yamlOptions = YamlOptions.Parse(yamlText);
            Assert.NotNull(yamlOptions);

            if (!string.IsNullOrWhiteSpace(expectedKey))
                Assert.AreEqual(expectedValue, yamlOptions.ReadValue<string>(expectedKey, ""));
        }
    }
}
