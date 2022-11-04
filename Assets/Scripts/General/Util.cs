using UnityEngine;

namespace General
{
    public static class Util
    {
        public static bool defaultBool = false;
        
        public delegate void DBool(bool b);
        public delegate void DVoid();
        public delegate void DInt(int i);
        public delegate void DFloat(float f);
        public delegate void DVector2(Vector2 vector);

        public static Vector2 absPerpendicular(Vector2 vector)
        {
            vector = Vector2.Perpendicular(vector);
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            return vector;
        }
        
    }
}