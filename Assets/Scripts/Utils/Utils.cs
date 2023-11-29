using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        public static Vector2 XY(this Vector3 vec3)
        {
            return new Vector2(vec3.x, vec3.y);
        }

        public static Vector3 ToVec3(this Vector2 vec2, float z)
        {
            return new Vector3(vec2.x, vec2.y, z);
        }
    }
}