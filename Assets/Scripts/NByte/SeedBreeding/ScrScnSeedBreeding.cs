using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NByte.SeedBreeding
{
    public class ScrScnSeedBreeding : ScrSceneBase
    {
        private static SeedBreedingGameData GameData => AppService.SeedBreeding;
        private static SeedBreedingConfig Config => GameData.Config;

        [SerializeField] private CinemachineVirtualCamera MovieCamera;
        [SerializeField] private ScrMoviePlayer MovVisitLab;
        [SerializeField] private ScrChessboard Chessboard;
        [SerializeField] private ScrPageTitleMain PageTitleMain;
        [SerializeField] private ScrPageTitleSecondary PageTitleSecondary;
        [SerializeField] private ScrPagePlayMain PagePlayMain;
        [SerializeField] private ScrPagePlaySecondary PagePlaySecondary;
        [SerializeField] private ScrPageFinishMain PageFinishMain;

        private int operations;
        public int Operations
        {
            get => operations;
            set
            {
                operations = value;
                PagePlayMain.Operations = operations;
                PagePlaySecondary.Operations = operations;
            }
        }

        public List<Eliminate> Eliminates { get; private set; } = new();

        private int points;
        public int Points
        {
            get => points;
            set
            {
                points = value;
                PagePlayMain.Points = points;
                PagePlaySecondary.Points = points;
                Eliminates.ForEach(t =>
                {
                    switch (t.ChessmanType)
                    {
                        case ChessmanModel.ChessmanTypes.Red: PagePlaySecondary.ProgressRed = t.Progress; break;
                        case ChessmanModel.ChessmanTypes.Green: PagePlaySecondary.ProgressGreen = t.Progress; break;
                        case ChessmanModel.ChessmanTypes.Blue: PagePlaySecondary.ProgressBlue = t.Progress; break;
                        case ChessmanModel.ChessmanTypes.Yellow: PagePlaySecondary.ProgressYellow = t.Progress; break;
                        default: break;
                    }
                });
            }
        }

        private int timeRemaining;
        public int TimeRemaining
        {
            get => timeRemaining;
            set
            {
                timeRemaining = value;
                PagePlayMain.Timer = timeRemaining;
                PagePlaySecondary.Timer = timeRemaining;
            }
        }

        public void VisitLab()
        {
            //StartGame();
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.ShowCurtain();

                PageTitleMain.gameObject.SetActive(false);
                PageTitleSecondary.gameObject.SetActive(false);

                MovVisitLab.PrepareCamera();
                AppService.PlayMusic(Config.PlayMusic, Config.MusicVolume);

                yield return AppService.HideCurtain();

                MovVisitLab.gameObject.SetActive(true);
                yield return MovVisitLab.Play();
                MovVisitLab.gameObject.SetActive(false);

                StartGame();
            }
        }
        public void StartGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                PageTitleMain.gameObject.SetActive(false);
                PageTitleSecondary.gameObject.SetActive(false);
                PagePlayMain.gameObject.SetActive(true);
                PagePlaySecondary.gameObject.SetActive(true);
                PageFinishMain.gameObject.SetActive(false);

                Operations = 0;
                Points = 0;
                Eliminates.ForEach(t => t.Count = 0);
                TimeRemaining = Config.GameLifetime;
                PagePlaySecondary.Talk = string.Empty;
                yield return Chessboard.Prepare();
                StartCoroutine(Timer());
                StartCoroutine(Talk());
            }
        }
        private IEnumerator Timer()
        {
            while (TimeRemaining > 0)
            {
                yield return new WaitForSeconds(1);
                TimeRemaining--;
            }
            //todo wait for the ending of current elimating
            PagePlayMain.gameObject.SetActive(false);
            PageFinishMain.gameObject.SetActive(true);
            PageFinishMain.Points = Points;
            AppService.StopMusic();
        }
        private IEnumerator Talk()
        {
            for (int i = 0; i < Config.Talks.Count; i++)
            {
                yield return new WaitForSeconds(Config.Talks[i].Delay);
                PagePlaySecondary.Talk = Config.Talks[i].Talk;
            }
        }
        public void AddPoints(IEnumerable<ScrChessman> chessmen)
        {
            foreach (var group in chessmen.GroupBy(t => t.ChessmanModel.ChessmanType))
            {
                Eliminates.Single(t => t.ChessmanType == group.Key).Count += group.Count();
            }
            Points += chessmen.Count();
        }
        public void EndGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.ShowCurtain();
                Chessboard.Clear();
                PageFinishMain.gameObject.SetActive(false);
                PagePlaySecondary.gameObject.SetActive(false);
                PageTitleMain.gameObject.SetActive(true);
                PageTitleSecondary.gameObject.SetActive(true);
                yield return AppService.HideCurtain();
            }
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                MovVisitLab.Init(MovieCamera);
                MovVisitLab.gameObject.SetActive(false);
                Chessboard.Init(this);
                Eliminates = GameData.ChessmanModels.GroupBy(t => t.ChessmanType).Select(t => new Eliminate(t.Key)).ToList();

                PageTitleMain.gameObject.SetActive(true);
                PageTitleSecondary.gameObject.SetActive(true);
                PagePlayMain.gameObject.SetActive(false);
                PagePlaySecondary.gameObject.SetActive(false);
                PageFinishMain.gameObject.SetActive(false);

                yield return AppService.HideCurtain();
            }
        }

        public class Eliminate
        {
            public ChessmanModel.ChessmanTypes ChessmanType { get; private set; }

            private int count;
            public int Count
            {
                get => count;
                set
                {
                    count = value;
                    Progress = Mathf.Clamp01(value / (float)Config.DNAMax);
                }
            }

            public float Progress { get; private set; }

            public Eliminate(ChessmanModel.ChessmanTypes chessmanType)
            {
                ChessmanType = chessmanType;
            }
        }
    }
}