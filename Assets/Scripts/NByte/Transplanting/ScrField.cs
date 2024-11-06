using UnityEngine;
using UnityEngine.EventSystems;

namespace NByte.Transplanting
{
    public class ScrField : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private GameObject Grass;
        [SerializeField] private GameObject Obstacle;
        [SerializeField] private GameObject Seedling;
        [SerializeField] private GameObject Start;

        public FieldValue FieldValue { get; private set; }

        public void Init(FieldValue fieldValue)
        {
            FieldValue = fieldValue;
            Load();
        }
        private void Load()
        {
            Grass.SetActive(!FieldValue.IsObstacle);
            Obstacle.SetActive(FieldValue.IsObstacle);
            Seedling.SetActive(false);
            Start.SetActive(FieldValue.IsOrigin);
        }

        public void Transplant()
        {
            Grass.SetActive(false);
            Obstacle.SetActive(false);
            Seedling.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}