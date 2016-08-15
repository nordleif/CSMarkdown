using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pechkin;
using Pechkin.Synchronized;

namespace CSMarkdown.Rendering
{
    public class PdfRenderer
    {
        public PdfRenderer()
        {

        }

        public byte[] Render(string html, PdfRendererOptions options)
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));

            if (options == null)
                options = new PdfRendererOptions();

            var config = new GlobalConfig()
                .SetMargins(options.MarginTop, options.MarginRight, options.MarginBottom, options.MarginLeft)
                .SetDocumentTitle("CSMarkdown")
                .SetPaperSize(PaperKind.A4);

            var pechkin = new SynchronizedPechkin(config);
            pechkin.Begin += OnBegin;
            pechkin.Error += OnError;
            pechkin.Warning += OnWarning;
            pechkin.PhaseChanged += OnPhase;
            pechkin.ProgressChanged += OnProgress;
            pechkin.Finished += OnFinished;

            var objectConfig = new ObjectConfig()
                .SetPrintBackground(true)
                .SetScreenMediaType(true);
            
            var result = pechkin.Convert(objectConfig, html);

            return result;
        }

        private void OnBegin(SimplePechkin converter, int expectedPhaseCount)
        {

        }

        private void OnError(SimplePechkin converter, string errorText)
        {
            Debug.WriteLine(errorText);
        }

        private void OnFinished(SimplePechkin converter, bool success)
        {
            
        }

        private void OnPhase(SimplePechkin converter, int phaseNumber, string phaseDescription)
        {
            Debug.WriteLine(phaseDescription);
        }

        private void OnProgress(SimplePechkin converter, int progress, string progressDescription)
        {
            Debug.WriteLine(progressDescription);
        }

        private void OnWarning(SimplePechkin converter, string warningText)
        {
            Debug.WriteLine(warningText);
        }
    }
}
