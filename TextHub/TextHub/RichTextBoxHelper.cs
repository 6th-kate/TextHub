using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TextHub
{
    /// <summary>
    /// Helps to bind the text of the RichTextBox to the ViewModel property
    /// </summary>
    class RichTextBoxHelper : DependencyObject
    {
        /// <summary>
        /// Protects the RichTextBoxHelper not to make it into the infinite loop as methods may have references on themselves
        /// </summary>
        private static List<Guid> recursionProtection = new List<Guid>();

        /// <summary>
        /// Gets the RichTextBox content
        /// </summary>
        /// <param name="obj">The RichTextBox</param>
        /// <returns>The RichTextBox content in a string</returns>
        public static string GetDocumentRTF(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentRTFProperty);
        }

        /// <summary>
        /// Sets the RichTextBox content
        /// </summary>
        /// <param name="obj">The RichTextBox</param>
        /// <param name="value">The content in .rtf in a string to set to the RichTextBox</param>
        public static void SetDocumentRTF(DependencyObject obj, string value)
        {
            var fw1 = (FrameworkElement)obj;
            if (fw1.Tag == null || (Guid)fw1.Tag == Guid.Empty)
                fw1.Tag = Guid.NewGuid();
            recursionProtection.Add((Guid)fw1.Tag);
            obj.SetValue(DocumentRTFProperty, value);
            recursionProtection.Remove((Guid)fw1.Tag);
        }

        /// <summary>
        /// As WPF RichTextBox content is not bindable, simulates the binding process for the RichTextBox content in a string
        /// </summary>
        public static readonly DependencyProperty DocumentRTFProperty = DependencyProperty.RegisterAttached(
            "DocumentRTF",
            typeof(string),
            typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata(
                "",
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (obj, e) =>
                {
                    var richTextBox = (RichTextBox)obj;
                    // Return if the recursion is found.
                    if (richTextBox.Tag != null && recursionProtection.Contains((Guid)richTextBox.Tag))
                        return;
                    
                    try
                    {
                        string docRtf = GetDocumentRTF(richTextBox);
                        FlowDocument doc = new FlowDocument();
                        if (!string.IsNullOrEmpty(docRtf))
                        {
                            using (var stream = new MemoryStream(Encoding.Default.GetBytes(docRtf)))
                            {
                                TextRange textRange = new TextRange(doc.ContentStart, doc.ContentEnd);
                                textRange.Load(stream, DataFormats.Rtf);
                            }                           
                        }
                        // Set the document
                        richTextBox.Document = doc;
                    }
                    catch (Exception)
                    {
                        richTextBox.Document = new FlowDocument();
                    }

                    // When the document changes updates the source.
                    richTextBox.TextChanged += (obj2, e2) =>
                    {
                        RichTextBox richTextBox2 = obj2 as RichTextBox;
                        if (richTextBox2 != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                                range.Save(ms, DataFormats.Rtf);
                                ms.Seek(0, SeekOrigin.Begin);
                                using (StreamReader sr = new StreamReader(ms))
                                {
                                    SetDocumentRTF(richTextBox, sr.ReadToEnd());
                                }
                                
                            }
                        }
                    };
                }
            )
        );
    }
}

