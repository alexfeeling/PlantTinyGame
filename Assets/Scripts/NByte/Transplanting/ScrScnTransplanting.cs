using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NByte.Transplanting
{
    public class ScrScnTransplanting : ScrSceneBase
    {
        private static TransplantingConfig Config => AppService.Transplanting.Config;

        [SerializeField] private Transform FieldsRoot;
        [SerializeField] private ScrMachine Machine;
        [SerializeField] private ScrRoute Route;
        [SerializeField] private ScrRut Rut;
        [SerializeField] private ScrPageTitleMain PageTitleMain;
        [SerializeField] private ScrPageTitleSecondary PageTitleSecondary;
        [SerializeField] private ScrPageTutorialMain PageTutorialMain;
        [SerializeField] private ScrPageTutorialSecondary PageTutorialSecondary;
        [SerializeField] private ScrPagePlayMain PagePlayMain;
        [SerializeField] private ScrPagePlaySecondary PagePlaySecondary;
        [SerializeField] private ScrPageFinishMain PageFinishMain;
        [SerializeField] private ScrPageFinishSecondary PageFinishSecondary;

        private int timer;
        public int Timer
        {
            get => timer;
            set
            {
                timer = value;
                PagePlaySecondary.Timer = value;
            }
        }

        private int points;
        public int Points
        {
            get => points;
            set
            {
                points = value;
                PagePlaySecondary.Points = value;
            }
        }

        private int difficulty;
        public int Difficulty
        {
            get => difficulty;
            set
            {
                difficulty = value;
                PagePlaySecondary.Difficulty = difficulty;
            }
        }

        private int progress;
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                PagePlaySecondary.Progress = progress;
            }
        }

        private List<ScrField> Fields { get; set; } = new();
        public List<ScrField> RoutePoints { get; set; } = new();
        public List<ScrField> RutPoints { get; set; } = new();
        public bool GameState { get; set; }
        public bool InputState { get; set; }
        public bool IsPlanning { get; set; }

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
                PageTutorialMain.gameObject.SetActive(false);
                PageTutorialSecondary.gameObject.SetActive(false);
                PagePlayMain.gameObject.SetActive(true);
                PagePlaySecondary.gameObject.SetActive(true);
                PageFinishMain.gameObject.SetActive(false);
                PageFinishSecondary.gameObject.SetActive(false);
                AppService.PlayMusic(Config.Music);
                Timer = Config.GameLifeTime;
                Points = 0;
                Difficulty = 1;
                Progress = 1;
                GameState = true;
                yield return StartTransplanting();
                yield return AppService.HideCurtain();
                StartCoroutine(StartTimer());
            }
        }
        private IEnumerator StartTimer()
        {
            while (Timer > 0)
            {
                yield return new WaitForSeconds(1);
                Timer--;
            }
            Finish();
        }
        public void StopGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.ShowCurtain();
                Machine.gameObject.SetActive(false);
                PageTitleMain.gameObject.SetActive(true);
                PageTitleSecondary.gameObject.SetActive(true);
                PageFinishMain.gameObject.SetActive(false);
                PageFinishSecondary.gameObject.SetActive(false);
                AppService.StopMusic();
                Fields.ForEach(t => Destroy(t.gameObject));
                Fields.Clear();
                RoutePoints.Clear();
                Route.RoutePoints = RoutePoints;
                RutPoints.Clear();
                Rut.RutPoints = RutPoints;
                yield return PreviewFields();
                yield return AppService.HideCurtain();
            }
        }
        public void Return()
        {

        }

        public IEnumerator StartTransplanting()
        {
            Fields.ForEach(t => Destroy(t.gameObject));
            Fields.Clear();
            RoutePoints.Clear();
            Route.RoutePoints = RoutePoints;
            RutPoints.Clear();
            Rut.RutPoints = RutPoints;
            List<FieldValue> fieldValues = BuildFieldValues();
            while (true)
            {
                RandomObstacles(fieldValues);
                if (CheckFieldValues(fieldValues.Single(t => t.Row == 0 && t.Column == 0), fieldValues.Where(t => !t.IsObstacle)))
                {
                    break;
                }
            }
            yield return CreateFields(fieldValues);
            ScrField field = Fields.Single(t => t.FieldValue.IsOrigin);
            RoutePoints.Add(field);
            RutPoints.Add(field);
            Machine.gameObject.SetActive(true);
            Machine.transform.position = field.transform.position;
            Machine.MachineState = ScrMachine.MachineStates.Idle;
            InputState = true;
        }
        private List<FieldValue> BuildFieldValues()
        {
            List<FieldValue> fieldValues = new();
            for (int i = 0; i < Config.LandSize.y; i++)
            {
                for (int j = 0; j < Config.LandSize.x; j++)
                {
                    fieldValues.Add(new(i, j));
                }
            }
            fieldValues.Single(t => t.Row == 0 && t.Column == 0).IsOrigin = true;
            return fieldValues;
        }
        private void RandomObstacles(List<FieldValue> fieldValues)
        {
            fieldValues.ForEach(t => t.IsObstacle = false);
            for (int i = 0; i < Config.ObstaclesMin + Difficulty - 1; i++)
            {
                fieldValues.Where(t => !t.IsOrigin && !t.IsObstacle).Random().IsObstacle = true;
            }
        }
        private bool CheckFieldValues(FieldValue currentField, IEnumerable<FieldValue> fieldValues)
        {
            List<FieldValue> enabledFields = new(fieldValues);
            enabledFields.Remove(currentField);
            List<FieldValue> nearbyFields = new();
            AddNearbyField(0, 1);
            AddNearbyField(0, -1);
            AddNearbyField(-1, 0);
            AddNearbyField(1, 0);
            if (nearbyFields.Count > 0)
            {
                for (int i = 0; i < nearbyFields.Count; i++)
                {
                    if (CheckFieldValues(nearbyFields[i], enabledFields))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return enabledFields.Count == 0;
            }

            void AddNearbyField(int rowOffset = 0, int columnOffset = 0)
            {
                int row = currentField.Row + rowOffset;
                int column = currentField.Column + columnOffset;
                if (enabledFields.SingleOrDefault(t => t.Row == row && t.Column == column) is FieldValue nearbyField)
                {
                    nearbyFields.Add(nearbyField);
                }
            }
        }
        private IEnumerator CreateFields(List<FieldValue> fieldValues)
        {
            Vector3 origin = new(-(Config.LandSize.x - 1) / 2 * Config.FieldSpacing, -(Config.LandSize.y - 1) / 2 * Config.FieldSpacing);
            for (int i = 0; i < fieldValues.Count; i++)
            {
                FieldValue fieldValue = fieldValues[i];
                Vector3 position = origin + new Vector3(fieldValue.Column * Config.FieldSpacing, fieldValue.Row * Config.FieldSpacing);
                AsyncOperationHandle<GameObject> asyncOperation = Config.FieldAsset.InstantiateAsync(position, Quaternion.identity, FieldsRoot);
                yield return asyncOperation;
                ScrField field = asyncOperation.Result.GetComponent<ScrField>();
                field.Init(this, fieldValue);
                Fields.Add(field);
            }
        }
        public void StopTransplanting()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                InputState = false;
                IsPlanning = false;
                Route.RoutePoints = null;
                for (int i = 1; i < RoutePoints.Count; i++)
                {
                    RoutePoints[i - 1].Transplant();
                    yield return Machine.Move(RoutePoints[i]);
                    RutPoints.Add(RoutePoints[i]);
                    Rut.RutPoints = RutPoints;
                }
                RoutePoints.Last().Transplant();
                yield return new WaitForSeconds(Config.CompleteDelay);
                yield return Config.CompleteAsset.InstantiateAsync();
                AppService.PlaySound(Config.CompleteSound);
                Points += Config.PointIncrement;
                Progress++;
                if (Progress > Config.DifficultySteps)
                {
                    Difficulty++;
                    Progress = 1;
                }
                if (Difficulty > Config.DifficultyMax)
                {
                    Finish();
                }
                else
                {
                    yield return StartTransplanting();
                }
            }
        }

        public void SetRoute(ScrField field)
        {
            bool routeChanged = false;
            if (!field.FieldValue.IsOrigin && !field.FieldValue.IsObstacle)
            {
                if (RoutePoints.Contains(field))
                {
                    if (RoutePoints.Last() == field)
                    {
                        RoutePoints.Remove(field);
                        routeChanged = true;
                    }
                }
                else
                {
                    ScrField last = RoutePoints.Last();
                    if (new Vector2(field.FieldValue.Row - last.FieldValue.Row, field.FieldValue.Column - last.FieldValue.Column).magnitude <= 1)
                    {
                        RoutePoints.Add(field);
                        routeChanged = true;
                    }
                }
            }
            if (routeChanged)
            {
                Route.RoutePoints = RoutePoints;
                if (RoutePoints.Count == Fields.Count(t => !t.FieldValue.IsObstacle))
                {
                    StopTransplanting();
                }
            }
        }
        public void ResetRoutes()
        {
            RoutePoints.RemoveAll(t => !t.FieldValue.IsOrigin);
            Route.RoutePoints = RoutePoints;
        }

        private IEnumerator PreviewFields()
        {
            Machine.gameObject.SetActive(false);
            Difficulty = Random.Range(3, 7);
            List<FieldValue> fieldValues = BuildFieldValues();
            RandomObstacles(fieldValues);
            yield return CreateFields(fieldValues);
        }
        private void Finish()
        {
            if (GameState)
            {
                PagePlayMain.gameObject.SetActive(false);
                PagePlaySecondary.gameObject.SetActive(false);
                PageFinishMain.gameObject.SetActive(true);
                PageFinishSecondary.gameObject.SetActive(true);
                GameState = false;
                InputState = false;
            }
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                PageTitleMain.gameObject.SetActive(true);
                PageTitleSecondary.gameObject.SetActive(true);
                PageTutorialMain.gameObject.SetActive(false);
                PageTutorialSecondary.gameObject.SetActive(false);
                PagePlayMain.gameObject.SetActive(false);
                PagePlaySecondary.gameObject.SetActive(false);
                PageFinishMain.Init(this);
                PageFinishMain.gameObject.SetActive(false);
                PageFinishSecondary.Init(this);
                PageFinishSecondary.gameObject.SetActive(false);
                yield return PreviewFields();
                yield return AppService.HideCurtain();
            }
        }

        private void Update()
        {
            Plan();
        }
        private void Plan()
        {
            if (GameState)
            {
                if (Input.GetMouseButtonDown(0) || (Input.touchSupported && Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    IsPlanning = true;
                }
                if (Input.GetMouseButtonUp(0) || (Input.touchSupported && Input.GetTouch(0).phase == TouchPhase.Ended))
                {
                    IsPlanning = false;
                }
            }
        }
    }
}