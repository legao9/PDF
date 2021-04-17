﻿namespace QuestPDF.Infrastructure
{
    public class TextStyle
    {
        internal string Color { get; set; } = "#000000";
        internal string FontType { get; set; } = "Helvetica";
        internal float Size { get; set; } = 12;
        internal float LineHeight { get; set; } = 1.2f;
        internal HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        internal FontWeight FontWeight { get; set; } = FontWeight.Normal;
        internal bool IsItalic { get; set; } = false;

        public static TextStyle Default => new TextStyle();
        
        public override string ToString()
        {
            return $"{Color}|{FontType}|{Size}|{LineHeight}|{Alignment}|{FontWeight}|{IsItalic}";
        }

        internal TextStyle Clone() => (TextStyle)MemberwiseClone();
    }
}