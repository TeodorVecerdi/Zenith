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
}
