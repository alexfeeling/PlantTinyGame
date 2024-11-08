using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NByte.Transplanting
{
    public class ScrField : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private List<GameObject> Obstacles;
        [SerializeField] private GameObject Unarrived;
        [SerializeField] private GameObject Arrived;
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
            Obstacles.ForEach(t => t.SetActive(false));
            if (FieldValue.IsObstacle)
            {
                Obstacles.Random().SetActive(true);
            }
            Unarrived.SetActive(true);
            Arrived.SetActive(false);
            Start.SetActive(false);
        }

        public void Transplant()
        {
            Unarrived.SetActive(false);
            Arrived.SetActive(true);
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