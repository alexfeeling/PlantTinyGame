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

        public int Difficulty { get; set; }
        public int Progress { get; set; }
        private List<ScrField> Fields { get; set; } = new();
        public List<ScrField> RoutePoints { get; set; } = new();
        public List<ScrField> RutPoints { get; set; } = new();
        public bool GameState { get; set; }
        public bool IsPlanning { get; set; }

        public void StartGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.ShowCurtain();
                PageTitleMain.gameObject.SetActive(false);
                AppService.PlayMusic(Config.Music);
                Difficulty = 1;
                Progress = 1;
                yield return AppService.HideCurtain();
                StartTransplanting();
            }
        }
        public void StopGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.ShowCurtain();
                Machine.gameObject.SetActive(false);
                PageTitleMain.gameObject.SetActive(true);
                AppService.StopMusic();
                Fields.ForEach(t => Destroy(t.gameObject));
                Fields.Clear();
                yield return AppService.HideCurtain();
            }
        }

        public void StartTransplanting()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
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
                GameState = true;
            }
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
                Fields.ForEach(t => Destroy(t.gameObject));
                Fields.Clear();
                RoutePoints.Clear();
                Route.RoutePoints = RoutePoints;
                RutPoints.Clear();
                Rut.RutPoints = RutPoints;
                Progress++;
                if (Progress > Config.DifficultySteps)
                {
                    Difficulty++;
                    Progress = 1;
                }
                StartTransplanting();
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

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                Machine.gameObject.SetActive(false);
                PageTitleMain.gameObject.SetActive(true);
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