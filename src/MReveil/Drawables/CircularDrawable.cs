using System.Diagnostics;

namespace Monbsoft.MReveil.Drawables;

internal class CircularDrawable : IDrawable
{
    public Color SecondColor { get; set; } = Colors.Red;
    public float Size { get; set; } = 20;
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
        float size = Size / 2;
        canvas.DrawArc(size, size, (dirtyRect.Width - Size), (dirtyRect.Height - Size), 90, secondAngle, true, false);
        var minuteAngle = 90 - (int)Math.Round(Minute * 6, MidpointRounding.AwayFromZero);
        canvas.StrokeColor = Colors.Orange;
        float size2 = Size + size;
        float size4 = size2 + size2;
        canvas.DrawArc(size2, size2, (dirtyRect.Width - size4), (dirtyRect.Height - size4), 90, minuteAngle, true, false);
    }
}
