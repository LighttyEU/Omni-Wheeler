using System;
using System.Collections.Generic;
using System.Drawing;

namespace Omni_Wheeler
{
    public static class RaceTrack
    {
        public static List<PointF> CreateSimpleTrack()
        {
            List<PointF> path = new List<PointF>();
            float spacing = 8f;

            AddLine(path, new PointF(250, 500), new PointF(520, 500), spacing);

            AddArc(path, new PointF(520, 420), 80, 90, 0, spacing);

            AddLine(path, path[path.Count - 1], new PointF(600, 250), spacing);

            AddArc(path, new PointF(520, 250), 80, 0, -90, spacing);

            AddLine(path, path[path.Count - 1], new PointF(300, 170), spacing);

            AddArc(path, new PointF(300, 300), 130, -90, -180, spacing);

            AddLine(path, path[path.Count - 1], new PointF(170, 420), spacing);

            AddArc(path, new PointF(250, 420), 80, 180, 90, spacing);

            return path;
        }

        public static List<PointF> CreateRaceTrackStyle()
        {
            List<PointF> path = new List<PointF>();
            float spacing = 7f;

            // Start unten, aber nicht ganz links
            AddLine(path, new PointF(360, 500), new PointF(520, 500), spacing);

            // Kurve unten rechts
            AddArc(path, new PointF(520, 420), 80, 90, 20, spacing);

            // Gerade nach rechts oben
            AddLine(path, path[path.Count - 1], new PointF(680, 300), spacing);

            // Große Kurve rechts oben
            AddArc(path, new PointF(590, 260), 100, 25, -100, spacing);

            // Lange Gegengerade nach links oben
            AddLine(path, path[path.Count - 1], new PointF(300, 150), spacing);

            // Linke obere Kurve
            AddArc(path, new PointF(300, 260), 110, -90, -190, spacing);

            // Gerade nach links unten
            AddLine(path, path[path.Count - 1], new PointF(170, 390), spacing);

            // Große Kurve unten links
            AddArc(path, new PointF(300, 390), 130, 180, 90, spacing);

            // Kurze Schlussgerade zurück zum Start
            AddLine(path, path[path.Count - 1], new PointF(360, 500), spacing);

            return path;
        }

        private static void AddLine(List<PointF> path, PointF start, PointF end, float spacing)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            int steps = Math.Max(1, (int)(length / spacing));

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;

                float x = start.X + t * dx;
                float y = start.Y + t * dy;

                path.Add(new PointF(x, y));
            }
        }

        private static void AddArc(List<PointF> path, PointF center, float radius, float startDeg, float endDeg, float spacing)
        {
            float startRad = startDeg * (float)Math.PI / 180f;
            float endRad = endDeg * (float)Math.PI / 180f;

            float arcLength = Math.Abs(endRad - startRad) * radius;
            int steps = Math.Max(1, (int)(arcLength / spacing));

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                float angle = startRad + t * (endRad - startRad);

                float x = center.X + radius * (float)Math.Cos(angle);
                float y = center.Y + radius * (float)Math.Sin(angle);

                path.Add(new PointF(x, y));
            }
        }
    }
}