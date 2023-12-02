using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class Util
    {
        public static Vector2 XY(this Vector3 vec3)
        {
            return new Vector2(vec3.x, vec3.y);
        }

        public static Vector3 ToVec3(this Vector2 vec2, float z)
        {
            return new Vector3(vec2.x, vec2.y, z);
        }
        
        public static IEnumerator Delay(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
            yield return null;
        }
    }
}