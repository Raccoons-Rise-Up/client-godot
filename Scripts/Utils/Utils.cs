using Godot;

namespace Client.Utilities 
{
    public class Utils 
    {
        public static float Remap(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s-a1)*(b2-b1)/(a2-a1);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float by)
        {
            float retX = Mathf.Lerp(a.x, b.x, by);
            float retY = Mathf.Lerp(a.y, b.y, by);
            return new Vector2(retX, retY);
        }
    }
}