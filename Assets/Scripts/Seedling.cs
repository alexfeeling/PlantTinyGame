using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AniYa.UI
{
    public class Seedling : MonoBehaviour
    {
        public GameObject[] TypeObjs;

        public void Refresh()
        {
            var pidx = GameManager.Instance.UsingGameData.plantIndex;
            for (int i = 0; i < TypeObjs.Length; i++)
            {
                var obj = TypeObjs[i];
                obj.SetActive(pidx == i);
            }
        }

    }
}
