using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using log4net;
using Tesseract;

namespace Profiling2.Ocr
{
    public class TesseractImageToText
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(TesseractImageToText));
        protected IDictionary<string, string> Texts { get; set; }
        protected IDictionary<string, float> MeanConfidences { get; set; }

        public TesseractImageToText(Stream stream, int? rotateDegrees, string language)
        {
            this.Texts = new Dictionary<string, string>();
            this.MeanConfidences = new Dictionary<string, float>();

            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            if (rotateDegrees.HasValue)
                switch (rotateDegrees.Value)
                {
                    case 90:
                        rotateFlipType = RotateFlipType.Rotate90FlipNone; break;
                    case 180:
                        rotateFlipType = RotateFlipType.Rotate180FlipNone; break;
                    case 270:
                        rotateFlipType = RotateFlipType.Rotate270FlipNone; break;
                    default:
                        rotateFlipType = RotateFlipType.RotateNoneFlipNone; break;
                }

            if (string.IsNullOrEmpty(language))
                foreach (string lang in new string[] { "eng", "fra" })
                    OcrScan(stream, lang, rotateFlipType);
            else
                OcrScan(stream, language, rotateFlipType);
        }

        public string Language
        {
            get
            {
                return this.MeanConfidences.Any() ? this.MeanConfidences.Aggregate((l, r) => l.Value > r.Value ? l : r).Key : null;
            }
        }

        public string Text
        {
            get
            {
                return string.IsNullOrEmpty(this.Language) ? null : this.Texts[this.Language].Trim();
            }
        }

        public float MeanConfidence
        {
            get
            {
                return string.IsNullOrEmpty(this.Language) ? 0 : this.MeanConfidences[this.Language];
            }
        }

        public bool PassedThreshold
        {
            get
            {
                return !string.IsNullOrEmpty(this.Text) && this.MeanConfidence > 0.7;
            }
        }

        // Coded from example located at https://github.com/charlesw/tesseract/blob/master/BaseApiTester/Program.cs
        protected void OcrScan(Stream stream, string language, RotateFlipType rotateFlipType)
        {
            try
            {
                using (TesseractEngine engine = new TesseractEngine(this.GetTessDataDir(), language, EngineMode.Default))
                {
                    using (Bitmap img = (Bitmap)Bitmap.FromStream(stream))
                    {
                        if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
                            img.RotateFlip(rotateFlipType);
                        using (var page = engine.Process(img))
                        {
                            this.Texts[language] = page.GetText();
                            this.MeanConfidences[language] = page.GetMeanConfidence();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Problem OCR scanning image stream.", e);
            }
        }

        protected string GetTessDataDir()
        {
            string tessDataDir = ConfigurationManager.AppSettings["TesseractDataDirectory"].ToString();
            if (string.IsNullOrEmpty(tessDataDir))
                tessDataDir = "./tessdata";
            return tessDataDir;
        }
    }
}
