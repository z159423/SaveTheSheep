using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain brain;
    [Space]
    [SerializeField] private CinemachineVirtualCamera mainMenuCMVM;

    [SerializeField] private CinemachineVirtualCamera inGameCamera_Ready;
    [SerializeField] private CinemachineVirtualCamera inGameCamera_Set;

    public static CameraManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void OnGameStart()
    {
        brain.m_DefaultBlend.m_Time = 0;

        mainMenuCMVM.Priority = 9;

        inGameCamera_Ready.Priority = 11;
        inGameCamera_Set.Priority = 10;

        StartCoroutine(waitTime());

        IEnumerator waitTime()
        {
            yield return new WaitForSeconds(0.7f);

            brain.m_DefaultBlend.m_Time = 0.7f;

            inGameCamera_Ready.Priority = 10;
            inGameCamera_Set.Priority = 11;
        }
    }

    public void OnGoToMainMenu()
    {
        brain.m_DefaultBlend.m_Time = 0;

        mainMenuCMVM.Priority = 12;
    }
}
