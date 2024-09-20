using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaCup.PixelGame.UtilTools;

internal static partial class Utils
{
    public static Vector2 BoxMuller(int mean, int dev, Random rnd)
    {
        double x, y, S;
        do
        {
            x = 2.0 * rnd.NextDouble() - 1.0;
            y = 2.0 * rnd.NextDouble() - 1.0;
            S = x * x + y * y;
        }
        while (S > 1.0 || S == 0);

        double fac = Math.Sqrt(-2.0 * Math.Log(S) / S);
        return new Vector2((float)((fac * x) * dev + mean), (float)((fac * y) * dev + mean));
    }

    public static Vector2 GetRandomPointInCircle(int radius, Random rnd)
    {
        var t = 2 * Math.PI * rnd.NextDouble();
        var u = rnd.NextDouble() + rnd.NextDouble();
        double r;
        if (u > 1) r = 2 - u;
        else r = u;
        return new Vector2((float)(radius * r * Math.Cos(t)), (float)(radius * r * Math.Sin(t)));
    }

    public static float RoundM(double n, double m) => (float)Math.Floor(((n + m - 1) / m) * m);
    public static Vector2 RoundM(Vector2 n, double m) => new Vector2(RoundM(n.X, m), RoundM(n.Y, m));

    public static Vector3 V2ToV3(Vector2 v ) => new Vector3(v.X, 0, v.Y);
}
