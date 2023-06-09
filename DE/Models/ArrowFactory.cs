﻿using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using DE.ViewModels;
using System;
using System.Text;

namespace DE.Models {
    public class ArrowFactory: Path {
        readonly static int arrow_width = 20;
        readonly static int arrow_height = 10;
        readonly static int rod_step = 5;

        Point start, end;
        int type;
        double diag;

        public Point StartPoint { get => start; set { if (start != value) { start = value; Update(); } } }
        public Point EndPoint { get => end; set { if (end != value) { end = value; Update(); } } }
        public int Type { get => type; set { if (type != value) { type = value; Update(); } } }

        public ArrowFactory() : base() {
            Stroke = Brushes.BurlyWood;
            StrokeThickness = 3;
            Fill = Brushes.Bisque;
            Update();
        }

        void UpdatePath() {
            int rod = type switch {
                0 or 1 => (int) diag - arrow_width,
                3 or 4 => (int) diag - arrow_width * 2,
                2 or 5 or _ => (int) diag,
            };
            bool R = true;

            StringBuilder sb = new();
            sb.Append($"M {(int) start.X},{(int) start.Y}");
            if (type == 1 || type == 2)
                while (rod > 0) {
                    int dist = rod > rod_step ? rod_step : rod;
                    rod -= dist;
                    sb.Append(R ? " l" : " m");
                    sb.Append($" {dist},0");
                    R = !R;
                }
            else sb.Append($" l {rod},0");

            int w = arrow_width, h = arrow_height;
            var head = type switch {
                0 or 1 => $" l 0,-{h} {w},{h} -{w},{h} 0,-{h}",
                3 or 4 => $" l {w},-{h} {w},{h} -{w},{h} -{w},-{h}",
                2 or 5 or _ => $" m -{w},-{h} l {w},{h} m -{w},{h} l {w},-{h}",
            };
            sb.Append(head);

            var c = Fill switch {
                ImmutableSolidColorBrush t => t.Color,
                SolidColorBrush t => t.Color,
                _ => new Color(),
            };
            Fill = new SolidColorBrush(new Color((byte)(type == 3 ? 0 : 255), c.R, c.G, c.B));

            try { Data = Geometry.Parse(sb.ToString()); }
            catch (Exception e) { Log.Write("Path error: " + e + "\nPath: " + sb.ToString()); }
        }

        void Update() {
            var delta = start - end;
            double new_diag = delta.Hypot();
            double orig_diag = new_diag > 0 ? new_diag : 0.001;
            if (new_diag < arrow_width * 1.5) new_diag = arrow_width * 1.5;
            diag = new_diag;

            UpdatePath();

            double angle = Math.Acos(delta.X / orig_diag);
            angle = angle * 180 / Math.PI;
            if (delta.Y < 0) angle = 360 - angle;
            angle = (angle + 180) % 360;

            RenderTransform = new RotateTransform(angle, (start.X - diag) / 2, (start.Y - arrow_height) / 2);
        }
    }
}
