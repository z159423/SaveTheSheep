using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MondayOFF;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public static bool gameStart {get; set;} = false;
    [field: SerializeField] public static bool stageFailed {get; private set;} = false;
    [field: SerializeField] public static bool enableDrawFence {get; set;} = true;
    public bool stageClear = false;

    [SerializeField] private Camera UICamera;
    [SerializeField] private LayerMask ActiveUIMask;
    [SerializeField] private LayerMask DeactiveUIMask;
    private bool UION = true;


    public static GameManager instance;

    private void Awake()
    {
        instance = this;

        Application.targetFrameRate = 60;

        //처음으로 유입된 사용자 이벤트 찍기
        if (!PlayerPrefs.HasKey("ClearZero"))
        {
            MondayOFF.EventTracker.ClearStage(0);
            PlayerPrefs.SetInt("ClearZero", 1);
        }
    }

    private void Start() {
        AdsManager.ShowBanner();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (UION)
            {
                UICamera.cullingMask = DeactiveUIMask;
                UION = false;
            }
            else
            {
                UICamera.cullingMask = ActiveUIMask;
                UION = true;
            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Screen.SetResolution(1242, 2688, false);

        }
    }

    //public GameObject InstantiateHelper(GameObject prefab)
    //{
    //    return Instantiate
    //}
}
