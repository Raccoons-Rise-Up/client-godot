using Godot;

namespace Client.Utilities 
{
    public class Utils 
    {
        public static float Remap(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s-a1)*(b2-b1)/(a2-a1);
        }

        public static Vector2 Lerp(Vector2 firstVector, Vector2 secondVector, float by)
        {
            float retX = Mathf.Lerp(firstVector.x, secondVector.x, by);
            float retY = Mathf.Lerp(firstVector.y, secondVector.y, by);
            return new Vector2(retX, retY);
        }
    }
}