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
            //阴天,
            雨天,
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
            GameManager.Instance.StopRain();

            _weatherIdx = 0;
            SetWeather(WeatherTypeEnum.雨天);
            skillCnt = 3;//技能只能用3次
            playTime = totalTime;

            //weatherChangeCD = weatherChangeTime;
            skillCntText.text = skillCnt.ToString();

            GameManager.Instance.ShowUnitPlants(false);


            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.PlayShakeRotate();
            }
            StopCoroutine("CorShowTalk");
            StartCoroutine("CorShowTalk");
        }

        private IEnumerator CorShowTalk()
        {
            ScreenUI.PlayRoleAnimation();

            var talkText = "天民田园通过水肥一体化智能灌溉系统给植物浇水，浇水时，要考虑气温、土壤温度和相对湿度等因素";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？早晨和傍晚是较为理想的浇水时间，避免在正午阳光强烈时浇水。";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？土壤表面出现干燥迹象时再进行浇水，避免过度浇水导致根部窒息。";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？现代农业常选择喷淋法或渗灌法，保证水分均匀地覆盖到植物根部周围";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "要记得根据天气情况，灵活选择浇水时间哦";
            GameManager.Instance.ShowTalk(talkText);

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
            {
                discreaseSpeed = 1f;
                weatherChangeCD = 30f;
            }
            else if (CurrWeatherType == WeatherTypeEnum.干旱)
            {
                discreaseSpeed = 2;
                weatherChangeCD = 15f;
            }
            else
            {
                discreaseSpeed = 0.2f;
                weatherChangeCD = 15f;
            }

            if (CurrWeatherType == WeatherTypeEnum.雨天)
                GameManager.Instance.PlayRain();
            else
                GameManager.Instance.StopRain();

            //设置天气显示效果
            weatherText.text = CurrWeatherType.ToString();
        }

        /*private void RandomWeather()
        {
            var widx = Random.Range(0, 2);
            SetWeather((WeatherTypeEnum)widx);
        }*/

        private WeatherTypeEnum[] _weatherQueue = new WeatherTypeEnum[] {
            WeatherTypeEnum.雨天,
            WeatherTypeEnum.晴天,
            WeatherTypeEnum.干旱,
            WeatherTypeEnum.雨天,
            WeatherTypeEnum.晴天,
            WeatherTypeEnum.干旱,
            WeatherTypeEnum.雨天,
            WeatherTypeEnum.晴天,
            WeatherTypeEnum.干旱,
            WeatherTypeEnum.雨天,
        };

        private int _weatherIdx;
        private void NextWeather()
        {
            _weatherIdx++;
            SetWeather(_weatherQueue[_weatherIdx]);
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


        private string _infoText = @"剩余时间

作物生长状态   {0}
环境湿度   {1}
土壤含水量   {2:0.}%
当前评价    {3}";

        private float _refreshInfoTime;

        private void RefreshInfo()
        {
            _refreshInfoTime += Time.deltaTime;
            if (_refreshInfoTime < 0.1f)
            {
                return;
            }
            var growRate = 0f;
            var rate = 0f;
            for (int i = 0; i < blockWaterDatas.Length; i++)
            {
                var bwd = blockWaterDatas[i];
                growRate += bwd.growTime / totalTime;
                rate += bwd.rate;
            }
            rate /= blockWaterDatas.Length;


            var evaluation = string.Empty;
            var evaRate = rate;

            if (evaRate >= 0.666f)
                evaluation = "优";
            else if (evaRate >= 0.333f)
                evaluation = "良";
            else
                evaluation = "差";

            var growthText = string.Empty;//（良好，一般，缓慢）
            if (growRate >= 0.666f)
                growthText = "良好";
            else if (evaRate >= 0.333f)
                growthText = "一般";
            else
                growthText = "缓慢";


            var wetnessText = string.Empty;//（根据天气产生变化，雨天-高，阴天-中，晴天-低）
            if (CurrWeatherType == WeatherTypeEnum.干旱)
                wetnessText = "低";
            else if (CurrWeatherType == WeatherTypeEnum.晴天)
                wetnessText = "中";
            //else if (CurrWeatherType == WeatherTypeEnum.阴天)
            //    wetnessText = "中";
            else if (CurrWeatherType == WeatherTypeEnum.雨天)
                wetnessText = "高";
            ScreenUI.InfoPanelText.text = string.Format(_infoText,
                growthText, wetnessText, rate * 100, evaluation);
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
                    pu.PlayChangeEffect();
                    pu.PlayScaleMotion();
                }


                weatherChangeCD -= Time.deltaTime;
                if (weatherChangeCD <= 0)
                {
                    NextWeather();
                    //weatherChangeCD = weatherChangeTime;
                    //RandomWeather();
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

                ScreenUI.TimeSlider.value = playTime / totalTime;
                RefreshInfo();

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
            StopCoroutine("CorShowTalk");
            GameManager.Instance.CloseTalk();

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

            GameManager.Instance.StopRain();
        }


    }

}