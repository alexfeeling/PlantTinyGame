using UnityEngine;
using UnityEngine.EventSystems;

namespace NByte.SeedBreeding
{
    public class ScrChessman : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private SeedBreedingGameData GameData => AppService.Instance.SeedBreeding;
        private SeedBreedingConfig Config => GameData.Config;

        [SerializeField] private Collider Collider;
        [SerializeField] private Rigidbody Rigidbody;

        public ScrChessboard Chessboard { get; private set; }
        public ChessmanModel ChessmanModel { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        private Vector3 TargetPosition { get; set; }

        private bool gravityState = true;
        public bool GravityState
        {
            get => gravityState;
            set
            {
                gravityState = value;
                Rigidbody.useGravity = value;
                Collider.enabled = value;
                if (!value)
                {
                    Rigidbody.velocity = Vector3.zero;
                    GotoTargetPosition();
                }
            }
        }

        public bool InputState { get; set; }

        private bool dragState;
        private bool DragState
        {
            get => dragState;
            set
            {
                dragState = value;
                if (value)
                {
                    gameObject.transform.localPosition = Chessboard.GetPointerPosition(TargetPosition);
                }
                else
                {
                    if (!Chessboard.Swap(this, gameObject.transform.localPosition - TargetPosition))
                    {
                        gameObject.transform.localPosition = TargetPosition;
                    }
                }
            }
        }

        public bool DeleteState { get; set; }

        public void Init(ScrChessboard chessboard, ChessmanModel chessmanModel, int row, int column)
        {
            Chessboard = chessboard;
            ChessmanModel = chessmanModel;
            Init(row, column);
        }
        public void Init(int row, int column)
        {
            Row = row;
            Column = column;
            TargetPosition = Chessboard.GetChessmanPosition(Row, Column);
            gameObject.name = $"{Row}, {Column}";
        }

        public void Move(int row, int column)
        {
            Init(row, column);
            GotoTargetPosition();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (InputState)
            {
                DragState = true;
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (InputState)
            {
                DragState = false;
            }
        }
        private void Update()
        {
            if (DragState)
            {
                gameObject.transform.localPosition = Chessboard.GetPointerPosition(TargetPosition);
            }
        }

        private void GotoTargetPosition()
        {
            gameObject.transform.localPosition = TargetPosition;
        }
    }
}