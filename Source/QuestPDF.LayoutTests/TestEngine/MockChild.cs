using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal class MockDrawingCommand
{
    public string MockId { get; set; }
    public int PageNumber { get; set; }
    public Position Position { get; set; }
    public Size Size { get; set; }
}

internal class ElementMock : Element
{
    public string MockId { get; set; }
    
    public float TotalWidth { get; set; }
    public float TotalHeight { get; set; }
    
    private float HeightOffset { get; set; }

    internal List<MockDrawingCommand> DrawingCommands { get; } = new();
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (TotalWidth > availableSpace.Width)
            return SpacePlan.Wrap();
        
        if (availableSpace.Height < Size.Epsilon)
            return SpacePlan.Wrap();

        var remainingHeight = TotalHeight - HeightOffset;

        if (remainingHeight < Size.Epsilon)
            return SpacePlan.FullRender(Size.Zero);
        
        if (remainingHeight > availableSpace.Height)
            return SpacePlan.PartialRender(TotalWidth, availableSpace.Height);
        
        return SpacePlan.FullRender(TotalWidth, remainingHeight);
    }

    internal override void Draw(Size availableSpace)
    {
        var height = Math.Min(TotalHeight - HeightOffset, availableSpace.Height);
        var size = new Size(TotalWidth, height);
        
        HeightOffset += height;
        
        Canvas.DrawRectangle(Position.Zero, size, Colors.Grey.Medium);
        
        if (Canvas is not SkiaCanvasBase canvasBase)
            return;

        var matrix = canvasBase.Canvas.TotalMatrix;
        
        DrawingCommands.Add(new MockDrawingCommand
        {
            MockId = MockId,
            PageNumber = PageContext.CurrentPage,
            Position = new Position(matrix.TransX / matrix.ScaleX, matrix.TransY / matrix.ScaleY),
            Size = availableSpace
        });

        if (HeightOffset > TotalHeight - Size.Epsilon)
            HeightOffset = 0;
    }
}