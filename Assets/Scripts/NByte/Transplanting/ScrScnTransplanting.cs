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

        public int Difficulty { get; set; }
        public int Progress { get; set; }
        private List<ScrField> Fields { get; set; }        
        public Stack<ScrField> RoutePoints { get; set; } = new();

        public void StartGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.ShowCurtain();
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
                //add start point
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
            currentField.RouteIndex = enabledFields.Count;
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
                field.Init(fieldValue);
                Fields.Add(field);
            }
        }
        public void StopTransplanting()
        {
            //clear fields and route points
        }

        protected override void Start()
        {
            base.Start();
            AppService.HideCurtain();
        }
    }
}