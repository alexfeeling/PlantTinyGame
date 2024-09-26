using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    //浇水UI
    public class WateringUI : BaseOperationUI
    {


        private enum WeatherTypeEnum
        {
            晴天,
            干旱,
            阴天
        }

        public Transform[] blockTranses;

        public Slider[] waterBars;

        public Text weatherText;
        public Text skillCntText;
        public Text scoreText;
        public Text timeText;

        private WeatherTypeEnum CurrWeatherType;

        [SerializeField]
        private float playTime = 0f;
        [SerializeField]
        private float totalTime = 90;
        [SerializeField]
        private float weatherChangeCD;
        [SerializeField]
        private float weatherChangeTime = 30;
        [SerializeField]
        private float discreaseSpeed;//水分下降速度，受天气影响
        [SerializeField]
        private float discreaseSpeedFactor = 1f;//水分下降速度系数

        private class PlantWaterData
        {
            public int index;
            public float waterValue;
            public int maxWater;
            /// <summary>
            /// 成长进度
            /// </summary>
            public float growTime;

            public float rate => waterValue / maxWater;

            public void AddWater(float water)
            {
                waterValue = Mathf.Clamp(waterValue + water, 0, maxWater);
            }

            public void Update(float delta, float discreaseSpeed)
            {
                var waterRate = waterValue / maxWater;
                if (waterRate >= 0.8f)
                {
                    growTime += delta;
                }
                else
                {
                    growTime += delta * 0.4f;
                }


                if (discreaseSpeed > 0)
                {
                    AddWater(-0.1f * discreaseSpeed * delta);
                }

            }

        }

        private List<int> _growUpIndexes;
        private float _growUpCountTime = 0;

        private PlantWaterData[] blockWaterDatas = new PlantWaterData[9];

        public int skillCnt = 0;

        public override void ShowStart()
        {
            base.ShowStart();

        }

        public void OnClickStart()
        {
            StartPlay();

            _growUpIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            _growUpCountTime = 0f;

            for (int i = 0; i < blockWaterDatas.Length; i++)
            {
                var bwd = new PlantWaterData();
                blockWaterDatas[i] = bwd;
                bwd.index = i;
                bwd.maxWater = 1;
                bwd.waterValue = Random.Range(0.4f, 0.6f);
            }
        }

        public override void StartPlay()
        {
            base.StartPlay();

            SetWeather(WeatherTypeEnum.晴天);
            skillCnt = 3;//技能只能用3次
            playTime = totalTime;

            weatherChangeCD = weatherChangeTime;
            skillCntText.text = skillCnt.ToString();

            GameManager.Instance.ShowUnitPlants(false);

            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.PlayShakeRotate();
            }
        }

        //机械化喷灌
        public void OnClickSkill()
        {
            if (isPlayingAnimation) return;
            if (skillCnt > 0)
            {
                skillCnt--;
                skillCntText.text = skillCnt.ToString();
                StartCoroutine(CorPlaySkill());
            }
        }

        private void SetWeather(WeatherTypeEnum weatherType)
        {
            CurrWeatherType = weatherType;

            if (CurrWeatherType == WeatherTypeEnum.晴天)
                discreaseSpeed = 1f;
            else if (CurrWeatherType == WeatherTypeEnum.干旱)
                discreaseSpeed = 2;
            else
                discreaseSpeed = 0.1f;

            //设置天气显示效果
            weatherText.text = CurrWeatherType.ToString();
        }

        private void RandomWeather()
        {
            var widx = Random.Range(0, 3);
            SetWeather((WeatherTypeEnum)widx);
        }

        private void WateringOn(int index)
        {
            if (isPlayingAnimation) return;
            StartCoroutine(CorWateringOn(index));
        }

        private IEnumerator CorWateringOn(int index)
        {
            var pUnit = GameManager.Instance.plantUnits[index];
            if (pUnit.IsPlayingAnimation)
            {
                yield break;
            }
            GameManager.Instance.plantUnits[index].PlayScaleMotion();
            isPlayingAnimation = true;
            yield return null;

            pUnit.PlayWateringAnimation();

            yield return new WaitForSeconds(1.2f);

            pUnit.StopWateringAnimation();

            var bwd = blockWaterDatas[index];
            bwd.AddWater(0.2f);

            isPlayingAnimation = false;
        }

        private IEnumerator CorPlaySkill()
        {
            isPlayingAnimation = true;

            yield return null;

            for (int i = 0; i < blockWaterDatas.Length; i++)
            {
                var bwd = blockWaterDatas[i];
                bwd.waterValue = bwd.maxWater;
            }

            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.PlayWateringAnimation();
            }

            yield return new WaitForSecondsRealtime(1.5f);

            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.StopWateringAnimation();
            }

            isPlayingAnimation = false;
        }

        protected override void OnClickPlantUnit(int index)
        {
            WateringOn(index);
        }

        protected override void Update()
        {
            base.Update();
            if (isPlaying)
            {
                _growUpCountTime += Time.deltaTime;
                if (_growUpCountTime >= 8f)
                {
                    _growUpCountTime -= 8;
                    var growUpIdx = _growUpIndexes.RandomPick(1)[0];
                    GameManager.Instance.ShowUnitPlant(growUpIdx, true);
                    var pu = GameManager.Instance.plantUnits[growUpIdx];
                    pu.PlayScaleMotion();
                }


                weatherChangeCD -= Time.deltaTime;
                if (weatherChangeCD <= 0)
                {
                    weatherChangeCD = weatherChangeTime;
                    RandomWeather();
                }

                for (int i = 0; i < blockWaterDatas.Length; i++)
                {
                    var beforeGrowthRate = blockWaterDatas[i].growTime / totalTime;
                    blockWaterDatas[i].Update(Time.deltaTime, discreaseSpeed * discreaseSpeedFactor);
                    var growthRate = blockWaterDatas[i].growTime / totalTime;
                    waterBars[i].value = blockWaterDatas[i].rate;

                    //if (beforeGrowthRate < 0.4f && growthRate >= 0.4f)
                    //{
                    //    GameManager.Instance.ShowUnitPlant(i, true);
                    //}
                }

                playTime -= Time.deltaTime;
                timeText.text = $"{playTime:0}秒";
                if (playTime <= 0)
                {
                    Finish();
                }

            }
        }


        public override void Finish(bool quit = false)
        {
            base.Finish(quit);
            if (!quit)
            {
                var sum = 0f;
                for (int i = 0; i < blockWaterDatas.Length; i++)
                {
                    var bwd = blockWaterDatas[i];
                    var rate = bwd.growTime / totalTime;
                    sum += rate;
                }
                sum /= blockWaterDatas.Length;

                var score = Mathf.RoundToInt(Mathf.Lerp(10, 60, sum));
                GameManager.Instance.UsingGameData.scoreWatering = score;

                scoreText.text = score.ToString();
                GameManager.Instance.SaveGame();
            }
        }


    }

}