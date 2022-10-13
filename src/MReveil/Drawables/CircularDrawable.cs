using System.Diagnostics;

namespace MReveil.Drawables;

internal class CircularDrawable : IDrawable
{
    public Color SecondColor { get; set; } = Colors.Red;
    public float Size { get; set; } = 10;
    public double Minute { get; set; }
    public double Second { get; set; }



    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Angle of the arc in degrees
        var secondAngle = 90 - (int)Math.Round(Second * 6, MidpointRounding.AwayFromZero);
        // Drawing code goes here
        // canvas.StrokeColor = Color.FromRgba("6599ff");
        canvas.StrokeColor = SecondColor;
        canvas.StrokeSize = Size;
        Debug.WriteLine($"The rect width is {dirtyRect.Width} and height is {dirtyRect.Height}");
        canvas.DrawArc(Size, Size, (dirtyRect.Width - 20), (dirtyRect.Height - 20), 90, secondAngle, true, false);
        var minuteAngle = 90 - (int)Math.Round(Minute * 6, MidpointRounding.AwayFromZero);
        canvas.StrokeColor = Colors.Orange;
        float size = Size + Size;
        canvas.DrawArc(size, size, (dirtyRect.Width - 40), (dirtyRect.Height - 40), 90, minuteAngle, true, false);
    }
}
