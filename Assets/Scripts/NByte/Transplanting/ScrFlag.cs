using UnityEngine;

namespace NByte.Transplanting
{
    public class ScrFlag : MonoBehaviour
    {
        [SerializeField] private GameObject Block;
        [SerializeField] private GameObject Up;
        [SerializeField] private GameObject Down;
        [SerializeField] private GameObject Left;
        [SerializeField] private GameObject Right;
        [SerializeField] private GameObject End;

        public void Init(FieldValue current, FieldValue next)
        {
            Block.SetActive(false);
            Up.SetActive(false);
            Down.SetActive(false);
            Left.SetActive(false);
            Right.SetActive(false);
            End.SetActive(true);

            End.SetActive(false);
            if (next == null)
            {
                Block.SetActive(true);
            }
            else if (next.Row > current.Row)
            {
                Up.SetActive(true);
            }
            else if (next.Row < current.Row)
            {
                Down.SetActive(true);
            }
            else if (next.Column < current.Column)
            {
                Left.SetActive(true);
            }
            else if (next.Column > current.Column)
            {
                Right.SetActive(true);
            }
            else
            {
                End.SetActive(true);
            }
        }
    }
}