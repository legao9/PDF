﻿using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    public interface ILine
    {
        
    }
    
    internal enum LineType
    {
        Vertical,
        Horizontal
    }
    
    internal class Line : Element, ILine, ICacheable
    {
        public LineType Type { get; set; }
        public string Color { get; set; }
        public float Size { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return Type switch
            {
                LineType.Vertical when availableSpace.Width >= Size => SpacePlan.FullRender(Size, 0),
                LineType.Horizontal when availableSpace.Height >= Size => SpacePlan.FullRender(0, Size),
                _ => SpacePlan.Wrap()
            };
        }

        internal override void Draw(Size availableSpace)
        {
            if (Type == LineType.Vertical)
            {
                Canvas.DrawRectangle(new Position(-Size/2, 0), new Size(Size, availableSpace.Height), Color);
            }
            else if (Type == LineType.Horizontal)
            {
                Canvas.DrawRectangle(new Position(0, -Size/2), new Size(availableSpace.Width, Size), Color);
            }
        }
    }
}