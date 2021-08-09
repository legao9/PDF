﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples.Engine
{
    public enum RenderingTestResult
    {
        Pdf,
        Images
    }
    
    public class RenderingTest
    {
        private string FileNamePrefix = "test";
        private Size Size { get; set; }
        private RenderingTestResult ResultType { get; set; } = RenderingTestResult.Images;
        
        private RenderingTest()
        {
            
        }

        public static RenderingTest Create()
        {
            return new RenderingTest();
        }

        public RenderingTest FileName([CallerMemberName] string fileName = "test")
        {
            FileNamePrefix = fileName;
            return this;
        }
        
        public RenderingTest PageSize(int width, int height)
        {
            Size = new Size(width, height);
            return this;
        }

        public RenderingTest ProducePdf()
        {
            ResultType = RenderingTestResult.Pdf;
            return this;
        }
        
        public RenderingTest ProduceImages()
        {
            ResultType = RenderingTestResult.Images;
            return this;
        }
        
        public void Render(Action<IContainer> content)
        {
            var container = new Container();
            content(container);
            
            var document = new SimpleDocument(container, Size);

            if (ResultType == RenderingTestResult.Images)
            {
                Func<int, string> fileNameSchema = i => $"{FileNamePrefix}-${i}.png";
                document.GenerateImages(fileNameSchema);
                Process.Start("explorer", fileNameSchema(0));
            }

            if (ResultType == RenderingTestResult.Pdf)
            {
                var fileName = $"{FileNamePrefix}.pdf";
                document.GeneratePdf(fileName);
                Process.Start("explorer", fileName);
            }
        }
    }
}