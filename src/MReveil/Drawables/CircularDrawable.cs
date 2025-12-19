using System.Diagnostics;

namespace Monbsoft.MReveil.Drawables;

public class CircularDrawable : IDrawable
{
    public Color SecondColor { get; set; } = Colors.Red;
    public Color MinuteColor { get; set; } = Colors.Orange;
    public Color HourColor { get; set; } = Colors.Blue;
    public float StrokeSize { get; set; } = 20;
    public double Hour { get; set; }
    public double Minute { get; set; }
    public double Second { get; set; }
    public bool ShowHours { get; set; } = false;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (dirtyRect.Width <= 0 || dirtyRect.Height <= 0)
            return;

        // Calcul des angles (0° = 12h, sens horaire)
        var secondAngle = 90 - (int)Math.Round(Second * 6, MidpointRounding.AwayFromZero);
        var minuteAngle = 90 - (int)Math.Round(Minute * 6, MidpointRounding.AwayFromZero);
        var hourAngle = 90 - (int)Math.Round(Hour * 30 + Minute * 0.5, MidpointRounding.AwayFromZero);

        float halfStroke = StrokeSize / 2;
        float doubleStroke = StrokeSize * 2;
        float tripleStroke = StrokeSize * 3;

        if (ShowHours)
        {
            // Cercle extérieur pour les secondes
            DrawArcSegment(
                canvas,
                SecondColor,
                StrokeSize,
                halfStroke,
                dirtyRect.Width - StrokeSize,
                dirtyRect.Height - StrokeSize,
                secondAngle
            );

            // Cercle du milieu pour les minutes
            DrawArcSegment(
                canvas,
                MinuteColor,
                StrokeSize,
                StrokeSize + halfStroke,
                dirtyRect.Width - doubleStroke - StrokeSize,
                dirtyRect.Height - doubleStroke - StrokeSize,
                minuteAngle
            );

            // Cercle intérieur pour les heures
            DrawArcSegment(
                canvas,
                HourColor,
                StrokeSize,
                doubleStroke + halfStroke,
                dirtyRect.Width - tripleStroke - doubleStroke,
                dirtyRect.Height - tripleStroke - doubleStroke,
                hourAngle
            );
        }
        else
        {
            // Mode chronomètre : seulement secondes et minutes
            // Cercle extérieur pour les secondes
            DrawArcSegment(
                canvas,
                SecondColor,
                StrokeSize,
                halfStroke,
                dirtyRect.Width - StrokeSize,
                dirtyRect.Height - StrokeSize,
                secondAngle
            );

            // Cercle intérieur pour les minutes
            DrawArcSegment(
                canvas,
                MinuteColor,
                StrokeSize,
                StrokeSize + halfStroke,
                dirtyRect.Width - doubleStroke - StrokeSize,
                dirtyRect.Height - doubleStroke - StrokeSize,
                minuteAngle
            );
        }
    }

    private void DrawArcSegment(
        ICanvas canvas,
        Color color,
        float strokeSize,
        float offset,
        float width,
        float height,
        int angle)
    {
        canvas.StrokeColor = color;
        canvas.StrokeSize = strokeSize;
        canvas.StrokeLineCap = LineCap.Round;

        canvas.DrawArc(
            offset,
            offset,
            width,
            height,
            90,
            angle,
            clockwise: true,
            closed: false
        );
    }
}
