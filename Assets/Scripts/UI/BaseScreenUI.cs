using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{


    /// <summary>
    /// 大屏幕UI基类
    /// </summary>
    public class BaseScreenUI : MonoBehaviour
    {

        public GameObject PanelStart;
        public GameObject PanelPlay;
        public GameObject PanelFinish;
        public Slider TimeSlider;
        public Text InfoPanelText;

        public enum RoleAnimTypeEnum
        {
            None,
            Boy,
            Girl
        }

        public RoleAnimTypeEnum roleAnimType;

        public void PlayRoleAnimation()
        {
            if (roleAnimType == RoleAnimTypeEnum.Boy)
            {
                GameManager.Instance.PlayBoyAnimation();
            }
            else if (roleAnimType == RoleAnimTypeEnum.Girl)
            {
                GameManager.Instance.PlayGirlAnimation();
            }
            else
            {
                GameManager.Instance.StopRoleAnimation();
            }
        }

    }

}
