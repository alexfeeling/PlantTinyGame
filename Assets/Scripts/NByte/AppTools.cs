using System.Collections.Generic;
using System.Linq;
using AniYa;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NByte
{
    public static class AppTools
    {
        public static T Random<T>(this IEnumerable<T> values)
        {
            int index = UnityEngine.Random.Range(0, values.Count());
            return values.ElementAt(index);
        }
        
    }
}