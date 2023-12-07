using System;
using System.Collections;
using System.Collections.Generic;
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

        public static IEnumerator DelayFrames(Action action, int frames)
        {
            while (frames > 0)
            {
                frames--;
                yield return null;
            }
            action.Invoke();
        }

        public static float SqrDistance(Vector2 vec, Vector2 other)
        {
            var num1 = vec.x - other.x;
            var num2 = vec.y - other.y;
            return num1 * num1 + num2 * num2;
        }

        public static IEnumerator CallbackCoroutine(this MonoBehaviour self, IEnumerator first, Action callback)
        {
            yield return self.StartCoroutine(first);
            callback.Invoke();
        }

        public static IEnumerator ChainCoroutines(IEnumerator first, IEnumerator second)
        {
            yield return first;
            yield return second;
        }
        
        public static IEnumerable<TK> Select<TK, T>(this IEnumerator<T> e, Func<T, TK> selector) {
            while (e.MoveNext()) {
                yield return selector.Invoke(e.Current);
            }
        }
    }
}