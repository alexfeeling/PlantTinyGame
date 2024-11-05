using UnityEngine;

namespace NByte.Transplanting
{
    public class ScrField : MonoBehaviour
    {
        [SerializeField] private ScrFlag Flag;

        public FieldValue FieldValue { get; private set; }

        public void Init(FieldValue fieldValue, FieldValue nextField)
        {
            FieldValue = fieldValue;
            Flag.Init(FieldValue, nextField);
        }
    }
}