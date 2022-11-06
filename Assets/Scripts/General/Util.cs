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

        public static float PIXEL = 1f / 8f;

        public static Vector2 absPerpendicular(Vector2 vector)
        {
            vector = Vector2.Perpendicular(vector);
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            return vector;
        }

        public static void SetLayerRecursively(this GameObject obj, int newLayer) {
            if (obj == null) {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform) {
                child.gameObject.SetLayerRecursively(newLayer);
            }
        }
    }
}
