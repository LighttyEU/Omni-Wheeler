using Omni_Wheeler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Omni_Wheeler
{

  public partial class MainForm : Form
  {
    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

    List<PointF> path;
    Vehicle vehicle;

    public MainForm()
    {
      DoubleBuffered = true;
      Width = 900;
      Height = 700;
      BackColor = Color.Black;

      path = CreateRaceTrack();

      vehicle = new Vehicle(150, 400);

      timer.Interval = 10;
      timer.Tick += Update;
      timer.Start();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.D1) vehicle.mode = DriveMode.Car;
      if (keyData == Keys.D2) vehicle.mode = DriveMode.Sideways;
      if (keyData == Keys.D3) vehicle.mode = DriveMode.Rotate;

      return base.ProcessCmdKey(ref msg, keyData);
    }

    private void Update(object sender, EventArgs e)
    {
      vehicle.FollowPath(path);
      vehicle.Update(0.01f);

      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      var g = e.Graphics;

      // Strecke zeichnen
      for (int i = 0; i < path.Count - 1; i++)
        g.DrawLine(Pens.White, path[i], path[i + 1]);

      vehicle.Draw(g);

      // HUD
      g.DrawString("1=Auto | 2=Omni | 3=Rotate",
          new Font("Arial", 12),
          Brushes.White,
          10, 10);
    }

    // ===================== STRECKE =====================

    List<PointF> CreateRaceTrack()
    {
      List<PointF> path = new List<PointF>();
      float spacing = 10f;  //Anzahl der Punkte in einer Linie oder Kurve => je größer, desto genauer, aber desto mehr Rechenaufwand

      AddLine(path, new PointF(150, 400), new PointF(650, 400), spacing);
      AddArc(path, new PointF(650, 300), 100, 90, -90, spacing);
      AddLine(path, new PointF(650, 200), new PointF(650, 100), spacing);
      AddArc(path, new PointF(400, 100), 250, 0, 180, spacing);
      AddLine(path, new PointF(150, 100), new PointF(150, 300), spacing);
      AddArc(path, new PointF(150, 300), 100, 180, 360, spacing);

      return path;
    }

    void AddLine(List<PointF> path, PointF start, PointF end, float spacing)
    {
      float dx = end.X - start.X;
      float dy = end.Y - start.Y;

      float length = (float)Math.Sqrt(dx * dx + dy * dy);
      int steps = (int)(length / spacing);

      for (int i = 0; i <= steps; i++)
      {
        float t = (float)i / steps;

        float x = start.X + t * dx;
        float y = start.Y + t * dy;

        path.Add(new PointF(x, y));
      }
    }

    void AddArc(List<PointF> path, PointF center, float radius, float startDeg, float endDeg, float spacing)
    {
      float startRad = startDeg * (float)Math.PI / 180f;
      float endRad = endDeg * (float)Math.PI / 180f;

      float arcLength = Math.Abs(endRad - startRad) * radius;
      int steps = (int)(arcLength / spacing);

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

  // ===================== FAHRZEUG =====================

  public enum DriveMode
  {
    Car,
    Sideways,
    Rotate
  }

  public class Vehicle
  {
    public PointF position;
    public float angle = 0;

    float size = 40;

    public DriveMode mode = DriveMode.Car;

    // Geschwindigkeiten
    float vx, vy, omega;

    // Auto-Modus
    float v = 80f;
    float L = 40f;
    float delta = 0f;

    public Vehicle(float x, float y)
    {
      position = new PointF(x, y);
    }

    public void FollowPath(List<PointF> path)
    {
      int closestIndex = GetClosestIndex(path);
      int lookIndex = Math.Min(closestIndex + 3, path.Count - 1); //vergleichts nächsten Punkt mit Letzten

      PointF target = path[lookIndex];

      float dx = target.X - position.X;
      float dy = target.Y - position.Y;

      float targetAngle = (float)Math.Atan2(dy, dx); //Atan2, weil genauer als atan
      float alpha = NormalizeAngle(targetAngle - angle);
      float distance = (float)Math.Sqrt(dx * dx + dy * dy);

      switch (mode)
      {
        case DriveMode.Car:
          CarMode(alpha, distance);
          break;

        case DriveMode.Sideways:
          SidewaysMode(dx, dy);
          break;

        case DriveMode.Rotate:
          RotateMode(alpha);
          break;
      }
    }

    void CarMode(float alpha, float distance)
    {
      delta = (float)Math.Atan2(2 * L * Math.Sin(alpha), distance);

      vx = v;
      vy = 0;
      omega = (v / L) * (float)Math.Tan(delta);
    }

    void SidewaysMode(float dx, float dy)
    {
      float cos = (float)Math.Cos(-angle); //angle negativ, weil world -> auto
      float sin = (float)Math.Sin(-angle);

      float localX = dx * cos - dy * sin;
      float localY = dx * sin + dy * cos;

      float k = 2.0f;

      vx = localX * k;
      vy = localY * k;
      omega = 0;
    }

    void RotateMode(float alpha)
    {
      vx = 0;
      vy = 0;

      float kRot = 3.0f;
      omega = alpha * kRot;
    }

    public void Update(float dt)
    {
      float cos = (float)Math.Cos(angle);
      float sin = (float)Math.Sin(angle);

      position.X += (vx * cos - vy * sin) * dt;
      position.Y += (vx * sin + vy * cos) * dt;

      angle += omega * dt;
    }

    public void Draw(Graphics g)
    {
      var corners = GetCorners();

      g.DrawPolygon(Pens.Red, corners.ToArray());

      foreach (var c in corners)
        g.FillEllipse(Brushes.Yellow, c.X - 4, c.Y - 4, 8, 8);
    }

    private List<PointF> GetCorners()
    {
      float h = size / 2;

      var local = new List<PointF>()
            {
                new PointF(-h, -h),
                new PointF(h, -h),
                new PointF(h, h),
                new PointF(-h, h)
            };

      var world = new List<PointF>();

      foreach (var p in local)
      {
        float x = (float)(p.X * Math.Cos(angle) - p.Y * Math.Sin(angle)); // angle positive weil auto -> world
        float y = (float)(p.X * Math.Sin(angle) + p.Y * Math.Cos(angle));

        world.Add(new PointF(position.X + x, position.Y + y));
      }

      return world;
    }

    private int GetClosestIndex(List<PointF> path)
    {
      float minDist = float.MaxValue;
      int index = 0;

      for (int i = 0; i < path.Count; i++)
      {
        float dx = path[i].X - position.X;
        float dy = path[i].Y - position.Y;
        float d = dx * dx + dy * dy;

        if (d < minDist)
        {
          minDist = d;
          index = i;
        }
      }

      return index;
    }

    private float NormalizeAngle(float a)
    {
      while (a > Math.PI) a -= 2 * (float)Math.PI;
      while (a < -Math.PI) a += 2 * (float)Math.PI;
      return a;
    }
  }
}