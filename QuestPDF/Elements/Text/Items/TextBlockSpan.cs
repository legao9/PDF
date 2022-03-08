﻿using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;
using Size = QuestPDF.Infrastructure.Size;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockSpan : ITextBlockItem
    {
        public string Text { get; set; }
        public TextStyle Style { get; set; } = new TextStyle();

        private Dictionary<(int startIndex, float availableWidth), TextMeasurementResult?> MeasureCache = new ();

        public virtual TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            var cacheKey = (request.StartIndex, request.AvailableWidth);
             
            if (!MeasureCache.ContainsKey(cacheKey))
                MeasureCache[cacheKey] = MeasureWithoutCache(request);
            
            return MeasureCache[cacheKey];
        }
        
        internal TextMeasurementResult? MeasureWithoutCache(TextMeasurementRequest request)
        {
            const char space = ' ';
            
            var paint = Style.ToPaint();
            var fontMetrics = Style.ToFontMetrics();

            var startIndex = request.StartIndex;
            
            if (request.IsFirstLineElement)
            {
                while (startIndex + 1 < Text.Length && Text[startIndex] == space)
                    startIndex++;
            }

            if (Text.Length == 0)
            {
                return new TextMeasurementResult
                {
                    Width = 0,
                    
                    LineHeight = Style.LineHeight ?? 1,
                    Ascent = fontMetrics.Ascent,
                    Descent = fontMetrics.Descent
                };
            }
            
            // start breaking text from requested position
            var text = Text.AsSpan().Slice(startIndex);
            
            var textLength = (int)paint.BreakText(text, request.AvailableWidth + Size.Epsilon);

            if (textLength <= 0)
                return null;

            if (textLength < text.Length && text[textLength] == space)
                textLength++;
            
            // break text only on spaces
            if (textLength < text.Length)
            {
                var lastSpaceIndex = text.Slice(0, textLength).LastIndexOf(space) - 1;

                if (lastSpaceIndex <= 0)
                {
                    if (!request.IsFirstLineElement)
                        return null;
                }
                else
                {
                    textLength = lastSpaceIndex + 1;
                }
            }

            text = text.Slice(0, textLength);

            var endIndex = startIndex + textLength;
            var nextIndex = endIndex;

            while (nextIndex + 1 < Text.Length && Text[nextIndex] == space)
                nextIndex++;
            
            // measure final text
            var finalText = text.TrimEnd();
            var width = paint.MeasureText(finalText);
            
            return new TextMeasurementResult
            {
                Width = width,
                
                Ascent = fontMetrics.Ascent,
                Descent = fontMetrics.Descent,
     
                LineHeight = Style.LineHeight ?? 1,
                
                StartIndex = startIndex,
                EndIndex = endIndex,
                NextIndex = nextIndex,
                TotalIndex = Text.Length
            };
        }
        
        public virtual void Draw(TextDrawingRequest request)
        {
            var fontMetrics = Style.ToFontMetrics();

            var text = Text.Substring(request.StartIndex, request.EndIndex - request.StartIndex);
            
            request.Canvas.DrawRectangle(new Position(0, request.TotalAscent), new Size(request.TextSize.Width, request.TextSize.Height), Style.BackgroundColor);
            request.Canvas.DrawText(text, Position.Zero, Style);

            // draw underline
            if ((Style.HasUnderline ?? false) && fontMetrics.UnderlinePosition.HasValue)
                DrawLine(fontMetrics.UnderlinePosition.Value, fontMetrics.UnderlineThickness ?? 1);
            
            // draw stroke
            if ((Style.HasStrikethrough ?? false) && fontMetrics.StrikeoutPosition.HasValue)
                DrawLine(fontMetrics.StrikeoutPosition.Value, fontMetrics.StrikeoutThickness ?? 1);

            void DrawLine(float offset, float thickness)
            {
                request.Canvas.DrawRectangle(new Position(0, offset - thickness / 2f), new Size(request.TextSize.Width, thickness), Style.Color);
            }
        }
    }
}