using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ShowOnce : ContainerElement, IStateResettable, ICacheable
    {
        private bool IsRendered { get; set; }

        public void ResetState(bool hardReset)
        {
            if (hardReset)
                IsRendered = false;
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Child == null || IsRendered)
                return SpacePlan.FullRender(0, 0);
            
            return base.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (Child == null || IsRendered)
                return;
            
            if (base.Measure(availableSpace).Type == SpacePlanType.FullRender)
                IsRendered = true;
            
            base.Draw(availableSpace);
        }
    }
}