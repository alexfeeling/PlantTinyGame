using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa
{


    public class PlantUnit : MonoBehaviour
    {

        public int index;

        public GameObject Empty;
        public GameObject Hoed;
        public GameObject HoedUnConver;
        public GameObject Plant;
        public GameObject Rock;
        public GameObject Fruit;
        public GameObject Insect;
        public GameObject Weed;
        public Animator WateringAnimator;
        public Animator ManuringAnimator;

        public GameObject[] ManuringEffects;

        public bool IsPlayingAnimation;

        public PlantPhase[] plantPhases;

        public Sprite[] FruitSps;

        public void ShowEmpty()
        {
            Empty.SetActive(true);
            Hoed.SetActive(false);
            HoedUnConver.SetActive(false);
            Plant.SetActive(false);
            Rock.SetActive(false);
            Fruit.SetActive(false);
            Insect.SetActive(false);
            Weed.SetActive(false);
        }

        public void PlayShakeRotate()
        {
            var seq = DOTween.Sequence();
            Plant.transform.localRotation = Quaternion.Euler(0, 0, -5);
            seq.Append(Plant.transform.DOLocalRotate(new Vector3(0, 0, 5), 1f));
            seq.Append(Plant.transform.DOLocalRotate(new Vector3(0, 0, -5), 1f));
            seq.SetLoops(-1);
            seq.SetTarget(Plant.transform);
        }

        public void StopShakeRotate()
        {
            Plant.transform.DOKill();
            Plant.transform.localRotation = Quaternion.identity;
        }

        public void SetScale(float scale, bool animate = false)
        {
            //Plant.transform.DOKill();
            if (animate)
                Plant.transform.DOScale(scale, 0.8f).SetEase(Ease.OutElastic, 1.2f);
            else
                Plant.transform.localScale = Vector3.one * scale;
        }

        public void ShowPlant(int plantId, bool isGrowth = true)
        {
            Empty.SetActive(false);
            Hoed.SetActive(true);
            HoedUnConver.SetActive(false);
            Rock.SetActive(false);

            Plant.SetActive(true);
            plantPhases[plantId].transform.localRotation = Quaternion.Euler(0, Random.Range(90, 180 + 90), 0);
            for (int i = 0; i < plantPhases.Length; i++)
            {
                plantPhases[i].gameObject.SetActive(i == plantId);
            }
            if (isGrowth)
                plantPhases[plantId].ShowGrowth();
            else
                plantPhases[plantId].ShowSeeling();
            Fruit.SetActive(false);
            Insect.SetActive(false);
            Weed.SetActive(false);
        }

        public void ShowPlantUncover(int plantId)
        {
            Empty.SetActive(false);
            Hoed.SetActive(false);
            HoedUnConver.SetActive(true);
            Plant.SetActive(true);
            Rock.SetActive(false);

            plantPhases[plantId].ShowSeeling();
            Fruit.SetActive(false);
            Insect.SetActive(false);
            Weed.SetActive(false);
        }

        public void ShowHoed()
        {
            Empty.SetActive(false);
            Hoed.SetActive(true);
            HoedUnConver.SetActive(false);
            Plant.SetActive(false);
            Rock.SetActive(false);
            Fruit.SetActive(false);
            Insect.SetActive(false);
            Weed.SetActive(false);
        }

        public void ShowHoedUncover()
        {
            Empty.SetActive(false);
            Hoed.SetActive(false);
            HoedUnConver.SetActive(true);
            Plant.SetActive(false);
            Rock.SetActive(false);
            Fruit.SetActive(false);
            Insect.SetActive(false);
            Weed.SetActive(false);
        }

        public void ShowRock()
        {
            Empty.SetActive(false);
            Hoed.SetActive(false);
            HoedUnConver.SetActive(true);
            Plant.SetActive(false);
            Rock.SetActive(true);
            Fruit.SetActive(false);
            Insect.SetActive(false);
            Weed.SetActive(false);
        }

        public void ShowUnitInsect()
        {
            Empty.SetActive(false);
            Hoed.SetActive(true);
            HoedUnConver.SetActive(false);
            Plant.SetActive(true);
            Rock.SetActive(false);
            Fruit.SetActive(false);
            Insect.SetActive(true);
            Weed.SetActive(false);
        }

        public void ShowUnitWeed()
        {
            Empty.SetActive(false);
            Hoed.SetActive(true);
            HoedUnConver.SetActive(false);
            Plant.SetActive(true);
            Rock.SetActive(false);
            Fruit.SetActive(false);
            Insect.SetActive(false);
            Weed.SetActive(true);
        }

        public void ClearUnitInsect()
        {
            Insect.SetActive(false);
        }

        public void clearUnitWeed()
        {
            Weed.SetActive(false);
        }

        public void ShowFruit(int fruitId)
        {
            if (fruitId == 0)
            {
                Fruit.SetActive(false);
            }
            else if (fruitId == 1)
            {
                Fruit.SetActive(true);
                Fruit.transform.localScale = Vector3.one;
                var plantId = GameManager.Instance.UsingGameData.plantIndex;
                var child = Fruit.transform.GetChild(0);
                child.GetComponent<Image>().sprite = FruitSps[plantId];
                child.DOKill();
                child.localPosition = Vector3.zero;
                child.DOLocalMoveY(20, 0.4f).SetEase(Ease.OutCubic).SetLoops(-1, LoopType.Yoyo);
            }
            else if (fruitId == 2)
            {
                Fruit.SetActive(true);
                var plantId = GameManager.Instance.UsingGameData.plantIndex;
                Fruit.transform.localScale = Vector3.one * 2;
                var child = Fruit.transform.GetChild(0);
                child.GetComponent<Image>().sprite = FruitSps[plantId];
                child.DOKill();
                child.localPosition = Vector3.zero;
                child.DOLocalMoveY(15, 0.3f).SetEase(Ease.OutCubic).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void PlayWateringAnimation()
        {
            WateringAnimator.gameObject.SetActive(true);
            WateringAnimator.Play("WateringAnimation");
            IsPlayingAnimation = true;
        }

        public void StopWateringAnimation()
        {
            WateringAnimator.Play("Empty");
            WateringAnimator.gameObject.SetActive(false);
            IsPlayingAnimation = false;
        }

        public void PlayManuringAnimation(int index)
        {
            ManuringAnimator.gameObject.SetActive(true);
            ManuringAnimator.Play("ManuringAnimation");
            IsPlayingAnimation = true;

            for (int i = 0; i < 4; i++)
            {
                ManuringEffects[i].SetActive(i == index);
            }

        }

        public void StopManuringAnimation()
        {
            ManuringAnimator.Play("Empty");
            ManuringAnimator.gameObject.SetActive(false);
            IsPlayingAnimation = false;
        }

        public void PlayScaleMotion()
        {
            transform.localScale = Vector3.one;
            transform.DOKill();
            //transform.DOScale(1, 1).SetEase(Ease.OutElastic, 1.2f);
            transform.DOPunchScale(new Vector3(0.2f, 0, 0.2f), 0.6f, elasticity: 0.2f);
            //transform.DOShakeScale(1f, new Vector3(1, 0, 1));
        }

    }

}