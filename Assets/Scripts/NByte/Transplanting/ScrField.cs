using UnityEngine;
using UnityEngine.EventSystems;

namespace NByte.Transplanting
{
    public class ScrField : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject Grass;
        [SerializeField] private GameObject Obstacle;
        [SerializeField] private GameObject Seedling;
        [SerializeField] private GameObject Start;

        public ScrScnTransplanting ScnTransplanting { get; private set; }
        public FieldValue FieldValue { get; private set; }

        public void Init(ScrScnTransplanting scnTransplanting, FieldValue fieldValue)
        {
            ScnTransplanting = scnTransplanting;
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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (ScnTransplanting != null && ScnTransplanting.GameState)
            {
                ScnTransplanting.SetRoute(this);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ScnTransplanting != null && ScnTransplanting.IsPlanning)
            {
                ScnTransplanting.SetRoute(this);
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            //if (ScnTransplanting != null && ScnTransplanting.IsPlanning)
            //{
            //    Debug.Log($"Exit On {gameObject.name}");
            //}
        }
    }
}