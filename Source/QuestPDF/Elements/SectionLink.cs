﻿using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class SectionLink : ContainerElement
    {
        public string SectionName { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            var targetSize = base.Measure(availableSpace);

            if (targetSize.Type == SpacePlanType.Wrap)
                return;

            var targetName = PageContext.GetDocumentLocationName(SectionName);
            Canvas.DrawSectionLink(targetName, targetSize);
            base.Draw(availableSpace);
        }
    }
}