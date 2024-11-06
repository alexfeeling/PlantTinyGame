using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NByte.ReturnToEarth
{
    public class ScrScnReturnToEarth : ScrSceneBase
    {
        private static ReturnToEarthGameData GameData => AppService.ReturnToEarth;
        private static ReturnToEarthConfig Config => GameData.Config;

        [SerializeField] private ScrSpaceship Spaceship;
        [SerializeField] private Vector3 StartPosition;
        [SerializeField] private Transform BoundaryTop;
        [SerializeField] private Transform BoundaryBottom;
        [SerializeField] private Transform MapsRoot;
        [SerializeField] private ScrPageTitleMain PageTitleMain;
        [SerializeField] private ScrPageTitleSecondary PageTitleSecondary;
        [SerializeField] private ScrPageTutorialMain PageTutorialMain;
        [SerializeField] private ScrPageTutorialSecondary PageTutorialSecondary;
        [SerializeField] private ScrPagePlayMain PagePlayMain;
        [SerializeField] private ScrPagePlaySecondary PagePlaySecondary;
        [SerializeField] private ScrPageWinMain PageWinMain;
        [SerializeField] private ScrPageWinSecondary PageWinSecondary;
        [SerializeField] private ScrPageLoseMain PageLoseMain;
        [SerializeField] private ScrPageLoseSecondary PageLoseSecondary;

        public bool State { get; private set; }

        private ScrMap CurrentMap { get; set; }
        private ScrMap NextMap { get; set; }

        public float DistanceToEarth { get; set; }
        private Coroutine LandingCoroutine { get; set; }

        public void StartTutorial()
        {
            PageTitleMain.gameObject.SetActive(false);
            PageTitleSecondary.gameObject.SetActive(false);
            PageTutorialMain.gameObject.SetActive(true);
            PageTutorialSecondary.gameObject.SetActive(true);
        }
        public void StartGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.ShowCurtain();

                DistanceToEarth = 0;
                foreach (MapModel mapModel in Config.Maps)
                {
                    switch (mapModel.MapType)
                    {
                        case MapModel.MapTypes.Takeoff:
                            DistanceToEarth += Config.MapDistance * (mapModel.LoopTimes - 0.5f);
                            break;
                        case MapModel.MapTypes.Sailing:
                            DistanceToEarth += Config.MapDistance * mapModel.LoopTimes;
                            break;
                        case MapModel.MapTypes.Landing:
                            break;
                        default:
                            break;
                    }
                }
                PagePlayMain.DistanceToEarth = DistanceToEarth;
                PagePlaySecondary.DistanceToEarth = DistanceToEarth;
                if (LandingCoroutine != null)
                {
                    StopCoroutine(LandingCoroutine);
                    LandingCoroutine = null;
                }

                ClearMap(CurrentMap);
                yield return CreateCurrentMap();
                PagePlaySecondary.Talk = CurrentMap.MapModel.MapInfo;
                ClearMap(NextMap);
                yield return CreateNextMap();
                AppService.PlayMusic(Config.PlayMusic);

                Spaceship.gameObject.SetActive(true);
                Spaceship.State = false;
                Spaceship.transform.position = StartPosition;

                PageTutorialMain.gameObject.SetActive(false);
                PageTutorialSecondary.gameObject.SetActive(false);
                PageLoseMain.gameObject.SetActive(false);
                PageLoseSecondary.gameObject.SetActive(false);
                PagePlayMain.gameObject.SetActive(true);
                PagePlaySecondary.gameObject.SetActive(true);
                State = true;

                yield return AppService.HideCurtain();

                yield return Spaceship.transform.DOMoveX(0, Config.SpaceshipFlyIn).SetEase(Ease.OutExpo).WaitForCompletion();
                yield return PagePlayMain.Countdown();
                Spaceship.State = true;
            }
        }
        public void EndGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.ShowCurtain();

                Spaceship.gameObject.SetActive(false);
                PageWinMain.gameObject.SetActive(false);
                PageWinSecondary.gameObject.SetActive(false);
                PageLoseMain.gameObject.SetActive(false);
                PageLoseSecondary.gameObject.SetActive(false);
                PageTitleMain.gameObject.SetActive(true);
                PageTitleSecondary.gameObject.SetActive(true);
                AppService.StopMusic();

                yield return AppService.HideCurtain();
            }
        }
        public void StopGame()
        {
            Spaceship.State = false;
            State = false;
            PagePlayMain.gameObject.SetActive(false);
            PagePlaySecondary.gameObject.SetActive(false);
            PageLoseMain.gameObject.SetActive(true);
            PageLoseSecondary.gameObject.SetActive(true);
        }
        public void Return()
        {
            AppTools.LoadStartScene("ScnReturnToEarth");
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                Vector3 corner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Mathf.Abs(Camera.main.transform.position.z)));
                BoundaryTop.transform.position = new(0, corner.y, 0);
                BoundaryTop.transform.localScale = new(corner.x, 1, 1);
                BoundaryBottom.transform.position = new(0, -corner.y, 0);
                BoundaryBottom.transform.localScale = new(corner.x, 1, 1);
                Spaceship.Ini(this);
                Spaceship.gameObject.SetActive(false);

                PageTitleMain.gameObject.SetActive(true);
                PageTitleSecondary.gameObject.SetActive(true);
                PageTutorialMain.gameObject.SetActive(false);
                PageTutorialSecondary.gameObject.SetActive(false);
                PagePlayMain.gameObject.SetActive(false);
                PagePlaySecondary.gameObject.SetActive(false);
                PageWinMain.gameObject.SetActive(false);
                PageWinSecondary.gameObject.SetActive(false);
                PageLoseMain.gameObject.SetActive(false);
                PageLoseSecondary.gameObject.SetActive(false);

                yield return AppService.HideCurtain();
            }
        }

        private void Update()
        {
            StartCoroutine(UpdateMap());
        }
        private IEnumerator UpdateMap()
        {
            if (State)
            {
                if (LandingCoroutine == null)
                {
                    DistanceToEarth = Mathf.Clamp(DistanceToEarth - Config.DistancePerTime * Time.deltaTime, 0, float.MaxValue);
                    PagePlayMain.DistanceToEarth = DistanceToEarth;
                    PagePlaySecondary.DistanceToEarth = DistanceToEarth;
                    if (DistanceToEarth == 0)
                    {
                        LandingCoroutine = StartCoroutine(Landing());
                    }
                }

                if (CurrentMap.transform.position.x < -Config.MapWidth - Config.MapDelay)
                {
                    ClearMap(CurrentMap);
                    CurrentMap = NextMap;
                    PagePlaySecondary.Talk = CurrentMap.MapModel.MapInfo;
                    yield return CreateNextMap();
                    NextMap.transform.position = CurrentMap.transform.position + new Vector3(Config.MapWidth, 0, 0);
                }
            }
        }
        private IEnumerator Landing()
        {
            Spaceship.State = false;
            yield return Spaceship.transform.DOMoveY(Config.LandingPosition, Config.LandingTime).SetEase(Ease.InOutExpo).WaitForCompletion();
            State = false;
            PagePlayMain.gameObject.SetActive(false);
            PagePlaySecondary.gameObject.SetActive(false);
            PageWinMain.gameObject.SetActive(true);
            PageWinSecondary.gameObject.SetActive(true);
        }

        private void ClearMap(ScrMap map)
        {
            if (map != null)
            {
                Destroy(map.gameObject);
            }
        }
        private IEnumerator CreateCurrentMap()
        {
            yield return CreateMap(1, 1, 0, t => CurrentMap = t);
        }
        private IEnumerator CreateNextMap()
        {
            if (CurrentMap.Step + 1 > CurrentMap.MapModel.LoopTimes)
            {
                if (CurrentMap.Stage + 1 <= Config.Maps.Count)
                {
                    yield return CreateMap(CurrentMap.Stage + 1, 1, CalcOffset(), t => NextMap = t);
                }
            }
            else
            {
                yield return CreateMap(CurrentMap.Stage, CurrentMap.Step + 1, CalcOffset(), t => NextMap = t);
            }

            float CalcOffset()
            {
                return CurrentMap.transform.position.x + Config.MapWidth;
            }
        }
        private IEnumerator CreateMap(int stage, int step, float offset, Action<ScrMap> setMap)
        {
            MapModel mapModel = Config.Maps[stage - 1];
            Vector3 position = new(offset, 0, 0);
            AsyncOperationHandle<GameObject> handle = mapModel.MapAsset.InstantiateAsync(position, Quaternion.identity, MapsRoot);
            yield return handle;
            ScrMap map = handle.Result.GetComponent<ScrMap>();
            map.Init(this, mapModel, stage, step);
            setMap(map);
        }
    }
}