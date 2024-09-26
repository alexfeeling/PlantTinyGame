using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AniYa
{

    /// <summary>
    /// 作物阶段显示
    /// </summary>
    public class PlantPhase : MonoBehaviour
    {

        public GameObject Seelding;
        public GameObject Growth;
        public GameObject Fruit;

        public void ShowSeeling()
        {
            Seelding.SetActive(true);
            Growth.SetActive(false);
            Fruit.SetActive(false);
        }

        public void ShowGrowth()
        {
            Seelding.SetActive(false);
            Growth.SetActive(true);
            Fruit.SetActive(false);
        }

        public void ShowFruit(int fruitId)
        {
            Seelding.SetActive(false);
            Growth.SetActive(true);
            if (fruitId == 0)
                Fruit.SetActive(false);
            else if (fruitId == 1)
            {
                Fruit.SetActive(true);
                Fruit.transform.localScale = Vector3.one;
            }
            else if (fruitId == 2)
            {
                Fruit.SetActive(true);
                Fruit.transform.localScale = Vector3.one * 2;
            }
        }

    }

}