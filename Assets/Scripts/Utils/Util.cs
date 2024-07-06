using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Program;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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

        public static IEnumerator CallbackCoroutine(IEnumerator first, Action callback)
        {
            yield return first;
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
        
        public static IEnumerable<T> GetAllComponents<T>()
        {
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (var obj in objects)
            {
                if (obj.scene.isLoaded && obj.TryGetComponent<T>(out var cmp))
                    yield return cmp;
            }
        }

        public static List<T> ListOf<T>(params T[] values) => values.ToList();
        
        public static System.Random Rng = new System.Random();

        public static IList<T> Shuffle<T>(this IEnumerable<T> sequence)
        {
            T swapTemp;
            var values = sequence.ToList();
            var currentlySelecting = values.Count;
            while (currentlySelecting > 1)
            {
                var selectedElement = Rng.Next(currentlySelecting);
                currentlySelecting--;
                if (currentlySelecting == selectedElement) continue;
                swapTemp = values[currentlySelecting];
                values[currentlySelecting] = values[selectedElement];
                values[selectedElement] = swapTemp;
            }

            return values;
        }
        
    }
}