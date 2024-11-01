using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AniYa.UI
{


    public class BaseOperationUI : MonoBehaviour
    {

        public GameObject PanelStart;
        public GameObject PanelPlay;
        public GameObject PanelFinish;

        public BaseScreenUI ScreenUI;

        protected bool isPlaying;
        protected bool isPlayingAnimation;

        public virtual void ShowStart()
        {
            gameObject.SetActive(true);

            PanelStart.SetActive(true);
            PanelPlay.SetActive(false);
            PanelFinish.SetActive(false);

            ScreenUI.PanelStart.SetActive(true);
            ScreenUI.PanelPlay.SetActive(false);
            ScreenUI.PanelFinish.SetActive(false);

            //ScreenUI.PlayRoleAnimation();
        }

        public virtual void StartPlay()
        {
            GameManager.Instance.StopRoleAnimation();

            PanelStart.SetActive(false);
            PanelPlay.SetActive(true);
            PanelFinish.SetActive(false);
            ScreenUI.PanelStart.SetActive(false);
            ScreenUI.PanelPlay.SetActive(true);
            ScreenUI.PanelFinish.SetActive(false);

            isPlaying = true;

            GameManager.Instance.RefreshUnitPlant();

        }

        private void OnEnable()
        {
            ScreenUI.gameObject.SetActive(true);
            PanelStart.SetActive(false);
            PanelPlay.SetActive(false);
            PanelFinish.SetActive(false);
        }

        protected virtual void OnDisable()
        {
            if (!ScreenUI.Equals(null))
                ScreenUI.gameObject.SetActive(false);
        }

        public virtual void Finish(bool quit = false)
        {
            isPlaying = false;
            PanelStart.SetActive(false);
            PanelPlay.SetActive(false);
            PanelFinish.SetActive(true);

            ScreenUI.PanelStart.SetActive(false);
            ScreenUI.PanelPlay.SetActive(false);
            ScreenUI.PanelFinish.SetActive(true);


            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.StopShakeRotate();
            }
        }

        public void Quit()
        {
            if (isPlaying)
                Finish(true);
            GameManager.Instance.ShowLogin();
        }

        protected virtual void OnClickPlantUnit(int index)
        {

        }

        protected virtual void Update()
        {
            if (isPlaying)
            {
                GameObject hitGO = null;
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        var ray = GameManager.Instance.OperationCamera.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(ray, out var hitInfo))
                        {
                            hitGO = hitInfo.collider.gameObject;
                        }
                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    var ray = GameManager.Instance.OperationCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out var hitInfo))
                    {
                        hitGO = hitInfo.collider.gameObject;
                    }
                }

                if (hitGO != null)
                {
                    if (hitGO.TryGetComponent<PlantUnit>(out var pu))
                    {
                        GameManager.Instance.PlayTouchSound();
                        OnClickPlantUnit(pu.index);
                    }
                }

            }
        }

    }

}