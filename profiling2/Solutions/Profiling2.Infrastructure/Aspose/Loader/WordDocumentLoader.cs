using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Aspose.Words;
using Aspose.Words.Properties;
using Aspose.Words.Saving;

namespace Profiling2.Infrastructure.Aspose.Loader
{
    public class WordDocumentLoader : BaseAsposeLoader
    {
        protected Document Document { get; set; }

        public WordDocumentLoader(Stream stream, string password)
            : base(stream)
        {
            try
            {
                LoadOptions loadOptions = new LoadOptions(LoadFormat.Auto, password, null);
                this.Document = new Document(stream, loadOptions);

                // try to strip password
                this.Document.WriteProtection.SetPassword(string.Empty);
                try
                {
                    this.Document.Unprotect(password);
                }
                catch (Exception e)
                {
                    this.Exception = e;
                }

                this.IsPasswordCorrect = true;
            }
            catch (Exception e)
            {
                this.Exception = e;
            }
        }

        public override Stream GetUnprotectedStream(Stream destination)
        {
            if (this.IsPasswordCorrect && this.Document != null)
            {
                this.Document.Save(destination, null);
                return destination;
            }
            return null;
        }

        /// <summary>
        /// Converts this Aspose Word Document into HTML.  Modified from Profiling1.
        /// </summary>
        /// <returns>Failure text on exception.</returns>
        public override Stream GetHtml(Stream destination)
        {
            if (this.Document != null)
            {
                HtmlSaveOptions options = new HtmlSaveOptions(SaveFormat.Html);
                //options.ImageSavingCallback = new HtmlImageExportCallback();
                options.ImagesFolder = ConfigurationManager.AppSettings["PreviewTempFolder"];
                options.ImagesFolderAlias = "Images";
                options.PrettyFormat = true;
                options.ExportTextInputFormFieldAsText = true;

                // TextBoxes render as images in HTML; replace node with plain text (PROF2-209) so that 
                // search terms may be highlighted.
                //
                // Note this works perfectly when the TextBox only contains text as in source ID 34581;
                // but in military documents such as source ID 388122, the context of the 'speech bubbles'
                // is lost among the image in the TextBox.
                foreach (Node n in this.Document.GetChildNodes(NodeType.Shape, true))
                {
                    string text = n.GetText();
                    if (!string.IsNullOrEmpty(text))
                    {
                        //Paragraph p = new Paragraph(this.Document);
                        Run r = new Run(this.Document, text);
                        //p.AppendChild(r);
                        CompositeNode parentParagraph = this.GetParentParagraph(n);
                        parentParagraph.InsertAfter(r, parentParagraph.FirstChild);
                        n.Remove();
                    }
                }

                this.Document.Save(destination, options);

                return destination;
            }
            throw new LoadSourceException(this.Exception.Message, this.Exception.InnerException);
        }

        protected CompositeNode GetParentParagraph(Node n)
        {
            if (n != null && n.ParentNode != null)
            {
                if (n.ParentNode.NodeType == NodeType.Paragraph)
                    return n.ParentNode;
                else
                    return this.GetParentParagraph(n.ParentNode);
            }
            return null;
        }

        public IDictionary<string, object> GetDocumentProperties()
        {
            IDictionary<string, object> props = new Dictionary<string, object>();
            if (this.Document != null)
            {
                foreach (DocumentProperty prop in this.Document.BuiltInDocumentProperties)
                {
                    props.Add(prop.Name, prop.Value);
                }
            }
            else
            {
                throw new LoadSourceException(this.Exception.Message, this.Exception.InnerException);
            }
            return props;
        }
    }
}
