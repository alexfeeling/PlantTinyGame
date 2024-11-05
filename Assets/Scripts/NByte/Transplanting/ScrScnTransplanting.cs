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
        [SerializeField] private ScrPageTitleMain PageTitleMain;

        private int blocks;
        private int Blocks
        {
            get => blocks;
            set
            {
                blocks = Mathf.Clamp(value, 0, Config.LandSize.x * Config.LandSize.y);
                PageTitleMain.Blocks = Blocks;
            }
        }

        private List<ScrField> Fields { get; set; } = new();

        public void IncreaseBlocks()
        {
            Blocks++;
        }
        public void DecreaseBlocks()
        {
            Blocks--;
        }

        public void StartGame()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                ClearFields();
                List<FieldValue> fieldValues;
                int timer = 1;
                while (true)
                {
                    fieldValues = RandomFieldValues();
                    if (CheckFieldValues(fieldValues.Single(t => t.Row == 0 && t.Column == 0), fieldValues.Where(t => !t.IsBlocked)))
                    {
                        Debug.Log($"Fields Successfully Created After {timer} Times.");
                        break;
                    }
                    timer++;
                    if (timer >= 100)
                    {
                        Debug.Log("Can't Create Fields. Try Again.");
                        break;
                    }
                }
                yield return CreateFields(fieldValues.Where(t => !t.IsBlocked).OrderByDescending(t => t.Step));
            }
        }
        private void ClearFields()
        {
            Fields.ForEach(t => Destroy(t.gameObject));
            Fields.Clear();
        }
        private List<FieldValue> RandomFieldValues()
        {
            List<FieldValue> fieldValues = new();
            for (int i = 0; i < Config.LandSize.y; i++)
            {
                for (int j = 0; j < Config.LandSize.x; j++)
                {
                    fieldValues.Add(new(i, j));
                }
            }
            FieldValue start = fieldValues.Single(t => t.Row == 0 && t.Column == 0);
            fieldValues.Remove(start);
            for (int i = 0; i < Blocks; i++)
            {
                fieldValues.Where(t => !t.IsBlocked).Random().IsBlocked = true;
            }
            fieldValues.Add(start);
            return fieldValues;
        }
        private bool CheckFieldValues(FieldValue currentField, IEnumerable<FieldValue> unreachedFields)
        {
            List<FieldValue> enabledFields = new(unreachedFields);
            enabledFields.Remove(currentField);
            currentField.Step = enabledFields.Count;
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
                return enabledFields.Count() == 0;
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
        private IEnumerator CreateFields(IEnumerable<FieldValue> fieldValues)
        {
            Vector3 origin = new(-(Config.LandSize.x - 1) / 2 * Config.FieldSpacing, -(Config.LandSize.y - 1) / 2 * Config.FieldSpacing);
            for (int i = 0; i < fieldValues.Count(); i++)
            {
                FieldValue fieldValue = fieldValues.ElementAt(i);
                Vector3 position = origin + new Vector3(fieldValue.Column * Config.FieldSpacing, fieldValue.Row * Config.FieldSpacing);
                AsyncOperationHandle<GameObject> asyncOperation = Config.FieldAsset.InstantiateAsync(position, Quaternion.identity, FieldsRoot);
                yield return asyncOperation;
                ScrField field = asyncOperation.Result.GetComponent<ScrField>();
                field.Init(fieldValue, fieldValues.ElementAtOrDefault(i + 1));
                Fields.Add(field);
            }
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                Blocks = 0;
                yield return AppService.HideCurtain();
            }
        }
    }
}

