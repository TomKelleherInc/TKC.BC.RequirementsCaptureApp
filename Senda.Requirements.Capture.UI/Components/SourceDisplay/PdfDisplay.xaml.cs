using DevExpress.Xpf.DocumentViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Senda.Requirements.Capture.UI.Components.SourceDisplay
{
    /// <summary>
    /// Interaction logic for PdfDisplay.xaml
    /// </summary>
    public partial class PdfDisplay : UserControl
    {

        public PdfDisplay()
        {
            InitializeComponent();
            pdfViewer.DetachStreamOnLoadComplete = true;

        }


        public void ClosePdf()
        {
            pdfViewer.CloseDocumentCommand.Execute(null);
        }


        public void LoadPdf(byte[] PdfBytes)
        {
            pdfViewer.DetachStreamOnLoadComplete = true;
            MemoryStream ms = new MemoryStream(PdfBytes);
            pdfViewer.DocumentSource = ms;
            pdfViewer.ZoomMode = DevExpress.Xpf.DocumentViewer.ZoomMode.FitToWidth;

            pdfViewer.SelectionEnded += PdfViewer_SelectionEnded;
            pdfViewer.SelectionContinued += PdfViewer_SelectionContinued;
        }

       public string GetDocumentText()
        {
            try
            {
                pdfViewer.SelectAll();
                string text = pdfViewer.GetSelectionContent().Text;
                pdfViewer.UnselectAll();
                return text;
            }
            catch (Exception ex)
            {
                return string.Empty;
                
            }
        }


        public void AnnotateCurrentlySelectedText(string AnnotationText, Color HighlightColor)
        {
            pdfViewer.HighlightSelectedText(AnnotationText, HighlightColor);
        }



        //public bool ContainsTerm(string searchTerm, bool IsWholeWord)
        //{
        //    if(IsWholeWord)
        //    {
        //        return Regex.Match(this.TextContents, $@"\b{searchTerm}\b", RegexOptions.IgnoreCase).Success;
        //    }
        //    else
        //    {
        //        return this.TextContents.Contains(searchTerm.ToLower());
        //    }
        //}


        private void PdfViewer_SelectionContinued(DependencyObject d, DevExpress.Xpf.PdfViewer.SelectionEventArgs e)
        {
             
        }

        private void PdfViewer_SelectionEnded(DependencyObject d, DevExpress.Xpf.PdfViewer.SelectionEventArgs e)
        {

        }

        public int CurrentPage
        {
            get
            {
                return pdfViewer.CurrentPageNumber;
            }
        }

        public string CurrentlySelectedPhrase
        {
            get
            {
                return pdfViewer.GetSelectionContent().Text;
            }
        }

        public TextSearchDirection SearchDirection;


        public void Find(string text, bool wholeWord, TextSearchDirection searchDirection)
        {
            var param = new TextSearchParameter()
            {
                Text = text,
                WholeWord = wholeWord,
                SearchDirection = searchDirection,
                IsCaseSensitive = false
            }; 

            pdfViewer.FindText(param);           
        }
        
 

    }
}
