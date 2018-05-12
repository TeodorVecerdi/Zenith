using System.IO;
using UnityEngine;

public class Utils {
    public const int CHUNKWIDTH = 48;
    public const int CHUNKHEIGHT = 32;
    /*
     * Maps a value in range (istart, istop) to range (ostart, ostop)
     * Code taken from Processing implementation
     */
    public static float map(float value, float istart, float istop, float ostart, float ostop) {
        return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
    }

    public static float RandomFloat(System.Random random, float start, float end) {
        return (float)random.NextDouble() * (end - start) + start;
    }
    public static int RandomInt(System.Random random, int start, int end) {
        return random.Next(start, end);
    }

    public static float Distance(Vector3 a, Vector3 b) {
        return Mathf.Sqrt(Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2));
    }

    public static float Distance(Vector2Int a, Vector2Int b) {
        Vector3 A = new Vector3(a.x, a.y, 0); 
        Vector3 B = new Vector3(b.x, b.y, 0);
        return Distance(A, B);
    }

    public static float Distance(Vector2Int a, Vector3 b) {
        Vector3 A = new Vector3(a.x, a.y, 0);
        return Distance(A, b);
    }

    public static float Distance(Vector3 a, Vector2Int b) {
        Vector3 B = new Vector3(b.x, b.y, 0);
        return Distance(a, B);
    }
    
}
