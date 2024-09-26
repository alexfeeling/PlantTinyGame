using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{


    public static T[] RandomSelect<T>(this T[] list, int count)
    {
        if (list.Length <= count)
            return list;

        var result = new T[count];

        var selLen = list.Length;
        for (int i = 0; i < count; i++)
        {
            var pickIdx = Random.Range(0, selLen);
            result[i] = list[pickIdx];
            list.Swap(pickIdx, selLen - 1);
            selLen--;
        }

        return result;
    }

    public static T[] RandomSelect<T>(this List<T> list, int count)
    {
        if (list.Count <= count)
            return list.ToArray();
        var result = new T[count];

        var selLen = list.Count;
        for (int i = 0; i < count; i++)
        {
            var pickIdx = Random.Range(0, selLen);
            result[i] = list[pickIdx];
            list.Swap(pickIdx, selLen - 1);
            selLen--;
        }

        return result;
    }

    public static T[] RandomPick<T>(this List<T> list, int count)
    {
        if (list.Count <= count)
            return list.ToArray();
        var result = new T[count];

        var selLen = list.Count;
        for (int i = 0; i < count; i++)
        {
            var pickIdx = Random.Range(0, selLen);
            result[i] = list[pickIdx];
            list.Swap(pickIdx, selLen - 1);
            selLen--;
        }
        list.RemoveRange(selLen, list.Count - selLen);
        return result;
    }
    
    public static void Swap<T>(this List<T> list, int index0, int index1)
    {
        var v1 = list[index0];
        list[index0] = list[index1];
        list[index1] = v1;
    }

    public static void Swap<T>(this T[] list, int index0, int index1)
    {
        var v1 = list[index0];
        list[index0] = list[index1];
        list[index1] = v1;
    }

}
