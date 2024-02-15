using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MondayOFF;

public class StageManager : MonoBehaviour
{

    [SerializeField] private Stage currentStage;

    [field: SerializeField] public Stage[] stageList;

    [SerializeField] private int currentStageNumber = 0;

    [Space]

    [SerializeField] private GameObject gameStartPanel;
    [SerializeField] private GameObject gameClearPanel;
    [SerializeField] private GameObject gameFailedPanel;

    [SerializeField] private Text timeText;
    [SerializeField] private Image timeFilled;

    [SerializeField] private Text stageText;
    [SerializeField] private Text clearStageText;

    [SerializeField] private ParticleSystem[] clearFanfare;


    [SerializeField] private GameObject invisibleGround;
    [SerializeField] private Slider fenceGagueSlider;

    private float currentTime = 7;
    private float timePercent = 0;

    [Space]

    [SerializeField] private float startTime = 7f;
    [SerializeField] private Text timerText;

    [SerializeField] private float valueForOneFence = 1f;
    [SerializeField] private float maxFenceValue = 30f;
    public float currentFenceGague { get; set; }

    public int thereStarValue = 20;
    public int twoStarValue = 10;

    [SerializeField] private GameObject thereStarObject;
    [SerializeField] private GameObject twoStarObject;
    [SerializeField] private Animator[] starAnimators;

    private int currentStar = 3;
    private Coroutine stageRestartCoroutin = null;

    [Space]

    public GameObject drawedFence;

    [Space]

    [SerializeField] private GameObject mainMenuDeco;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject mainMenuUIBundle;
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private bool enableDecoIslands = true;
    [SerializeField] private GameObject[] inGameDecoIslands;
    [SerializeField] private GameObject stageSelectMenuObject;
    [SerializeField] private StageSelectMenu stageSelectMenu;
    [SerializeField] private Transform playableStageParent;
    [SerializeField] private Animator gameClearAnimator;

    [SerializeField] private int selectedStageNum;
    private StageSelectNode selectedStageNode;

    public static StageManager instance;

    public bool isLoadStageData = false;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Animator timerAnimator;
    private bool randomStage = false;
    private int randomStageNum;

    private bool useFenceGague = true;
    private bool timerEnable = true;

    private void Awake()
    {
        instance = this;

        DataManager.LoadStageData(ref stageList);

        currentStageNumber = DataManager.LoadData<int>("currentStage");
        randomStageNum = DataManager.LoadData<int>("randomStageNum");
        randomStage = DataManager.LoadData<bool>("randomStage");
    }

    private void Start()
    {
        OnClickStageSelectNodeBtn(currentStageNumber, null);

        timerText.text = startTime.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            useFenceGague = !useFenceGague;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            timerEnable = !timerEnable;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ActiveInGameIsland();
        }

        if (GameManager.gameStart)
        {
            if (timerEnable)
                currentTime -= Time.deltaTime;

            timeText.text = (Mathf.Ceil(currentTime)).ToString();

            if (currentTime < 0)
                StageClear();

            timePercent = currentTime / startTime;
            timeFilled.fillAmount = 1 - timePercent;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ES3.DeleteKey("stageData");
            ES3.DeleteKey("currentStage");
            ES3.DeleteKey("randomStageNum");
            ES3.DeleteKey("randomStage");

            for (int i = 0; i < stageList.Length; i++)
            {
                stageList[i].stageData.openedStage = false;
            }

            stageList[0].stageData.openedStage = true;

            stageSelectMenu.ReInitAllGeneratedNode();
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            for (int i = 0; i < stageList.Length; i++)
            {
                stageList[i].stageData.openedStage = true;
            }

            stageSelectMenu.ReInitAllGeneratedNode();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RestartCurrentStage();
        }

        if (GameManager.gameStart)
        {
            timerAnimator.SetBool("Start", true);
        }
        else
        {
            timerAnimator.SetBool("Start", false);
        }
    }

    public void StageStart()
    {
        //TryStage
        gameStartPanel.SetActive(false);

        GameManager.gameStart = true;
        GameManager.enableDrawFence = false;

        invisibleGround.SetActive(false);
    }

    public void StageClear()
    {
        GameManager.gameStart = false;
        GameManager.enableDrawFence = true;
        GameManager.instance.stageClear = true;

        clearStageText.text = "STAGE";/* + currentStageNumber */;

        for (int i = 0; i < clearFanfare.Length; i++)
        {
            clearFanfare[i].Play();
        }

        StartCoroutine(ClearPanelOn());

        IEnumerator ClearPanelOn()
        {
            yield return new WaitForSeconds(2f);

            gameClearPanel.SetActive(true);

            yield return new WaitForSeconds(1f);

            if (currentStar == 3)
            {
                StartCoroutine(starAnimation(3));

                IEnumerator starAnimation(int count)
                {
                    for (int i = 0; i < count; i++)
                    {
                        yield return new WaitForSeconds(.5f);
                        starAnimators[i].SetTrigger("Pop");
                    }
                }

            }
            else if (currentStar == 2)
            {
                StartCoroutine(starAnimation(2));

                IEnumerator starAnimation(int count)
                {
                    for (int i = 0; i < count; i++)
                    {
                        yield return new WaitForSeconds(.5f);
                        starAnimators[i].SetTrigger("Pop");
                    }
                }

            }
            else if (currentStar == 1)
            {
                StartCoroutine(starAnimation(1));

                IEnumerator starAnimation(int count)
                {
                    for (int i = 0; i < count; i++)
                    {
                        yield return new WaitForSeconds(.5f);
                        starAnimators[i].SetTrigger("Pop");
                    }
                }
            }

            if (currentStage.stageData.score < currentStar)
                currentStage.stageData.score = currentStar;

            if (currentStageNumber + 1 < stageList.Length)
            {
                PlayerPrefs.SetInt("currentStage", currentStageNumber + 1);
                DataManager.SaveData<int>("currentStage", currentStageNumber + 1);

                stageList[currentStageNumber + 1].stageData.openedStage = true;
                stageSelectMenu.ReinitNode(currentStageNumber);
                stageSelectMenu.ReinitNode(currentStageNumber + 1);
                stageSelectMenu.SelectNode(currentStageNumber + 1);
            }
            else
            {
                //PlayerPrefs.SetInt("curerntStage", 0);

                print(currentStageNumber);
                stageSelectMenu.ReinitNode(currentStageNumber);
            }
        }

        SoundManager.instance.GenerateAudioAndPlaySFX("clapping", Vector3.zero);

        DataManager.SaveStageData(stageList);

        MondayOFF.EventTracker.ClearStage(currentStageNumber + 1);

        currentStage.stageData.clearStage = true;

        DataManager.SaveData<bool>("randomStage", randomStage);

        if (randomStage)
        {
            RandomStage();
            randomStageNum++;
            DataManager.SaveData<int>("randomStageNum", randomStageNum);
        }

        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
    }

    public void StageFailed()
    {
        if (GameManager.gameStart)
            stageRestartCoroutin = StartCoroutine(wait());

        GameManager.gameStart = false;

        IEnumerator wait()
        {
            yield return new WaitForSeconds(2f);

            if (!GameManager.gameStart)
                RestartCurrentStage();
        }
    }

    public void RestartCurrentStage()
    {
        if (drawedFence != null)
        {
            Destroy(drawedFence);
            drawedFence = null;
        }

        LoadCurrentStage();

        GameManager.gameStart = false;
        GameManager.enableDrawFence = true;
        GameManager.instance.stageClear = false;

        ResetStage();
    }

    public void LoadNextStage()
    {
        if (drawedFence != null)
        {
            Destroy(drawedFence);
            drawedFence = null;
        }

        //int notclearStageNum, notOpenStageNumber;

        if (randomStage)
        {
            print("0 : 랜덤스테이지 계속");
            RandomStage();
        }
        else if (currentStageNumber + 1 < stageList.Length)
        {
            print("1 : 다음스테이지로");
            NextSage();
            MondayOFF.EventTracker.TryStage(currentStageNumber + 1);
        }
        else
        {
            print("2 : 모든스테이지 오픈, 클리어 : 랜덤스테이지 시작");
            randomStageNum = currentStageNumber;
            randomStageNum++;
            RandomStage();

            // if (CheckAllStageOpened(out notOpenStageNumber))
            // {
            //     if (CheckAllStageClear(out notclearStageNum))
            //     {
            //         print("2 : 모든스테이지 오픈, 클리어 : 랜덤스테이지 시작");
            //         randomStageNum = currentStageNumber;
            //         randomStageNum++;
            //         RandomStage();
            //     }
            //     else
            //     {
            //         print("4 : 아직 클리어 안한 스테이지가 있어 해당 스테이지 플레이 : " + notclearStageNum);
            //         currentStageNumber = notclearStageNum;
            //         stageText.text = "STAGE " + (currentStageNumber + 1);
            //     }
            // }
            // else
            // {
            //     print("3 : 아직 클리어 안한 스테이지가 있어 해당 스테이지 플레이 : " + notOpenStageNumber);
            //     currentStageNumber = notOpenStageNumber;
            //     stageText.text = "STAGE " + (currentStageNumber + 1);
            // }
        }

        currentStage.gameObject.SetActive(false);

        currentStage = stageList[currentStageNumber];

        ResetStage();

        GameManager.instance.stageClear = false;

        void NextSage()
        {
            //다음 스테이지가 있으면 다음 스테이지로
            if (currentStageNumber + 1 < stageList.Length)
                currentStageNumber++;
            // 다음스테이지가 없으면 1번 스테이지부터
            else
                currentStageNumber = 1;

            stageText.text = "STAGE " + (currentStageNumber + 1);
        }

        CameraManager.instance.OnGameStart();
    }

    public void ResetStage()
    {
        if (stageRestartCoroutin != null)
            StopCoroutine(stageRestartCoroutin);

        LoadCurrentStage();

        currentTime = startTime;
        timeText.text = currentTime.ToString();

        timeFilled.fillAmount = 0;

        gameStartPanel.SetActive(true);
        gameFailedPanel.SetActive(false);
        gameClearPanel.SetActive(false);

        invisibleGround.SetActive(true);

        ChangeFenceGague(currentStage.fenceGagueThisStage);

        thereStarObject.SetActive(true);
        twoStarObject.SetActive(true);

        currentStar = 3;

        for (int i = 0; i < starAnimators.Length; i++)
        {
            starAnimators[i].SetTrigger("Reset");
        }

        
    }

    public void ChangeFenceGague(float value)
    {
        currentFenceGague = value;
        fenceGagueSlider.maxValue = currentFenceGague;
        fenceGagueSlider.value = currentFenceGague;

        thereStarValue = (int)(currentFenceGague / 1.5f);
        twoStarValue = (int)(currentFenceGague / 3);
    }

    public Sheep GetClosestSheep(Vector3 position)
    {
        return currentStage.GetClosestSheep(position);
    }

    public Hunter GetClosestWolf(Vector3 position)
    {
        return currentStage.GetClosestWolf(position);
    }

    public void UseFenceGague(float percent)
    {
        if (!useFenceGague)
            return;

        currentFenceGague -= valueForOneFence * percent;
        fenceGagueSlider.value = currentFenceGague;

        if (currentFenceGague < thereStarValue)
        {
            thereStarObject.SetActive(false);
            currentStar = 2;
        }

        if (currentFenceGague < twoStarValue)
        {
            twoStarObject.SetActive(false);
            currentStar = 1;
        }
    }

    public bool HaveFenceGague()
    {
        if (currentFenceGague >= valueForOneFence)
            return true;
        else
            return false;
    }

    //스테이지 넘버로 스테이지 시작
    private void StartStageAsNumber(int stageNum)
    {
        randomStage = false;
        currentStage = stageList[stageNum];
        currentStageNumber = stageNum;

        stageText.text = "STAGE " + (stageNum + 1);

        LoadCurrentStage();

        ChangeFenceGague(currentStage.fenceGagueThisStage);

        currentTime = startTime;

        MondayOFF.EventTracker.TryStage(currentStageNumber + 1);
    }

    //메인메뉴에서 게임 시작 버튼 클릭시
    public void OnClickStartButton()
    {
        ActiveMainMenu();

        if (randomStage)
        {
            StartStageAsNumber(currentStageNumber);
            randomStage = true;
            stageText.text = "STAGE " + (randomStageNum + 1);
        }
        else
            StartStageAsNumber(selectedStageNum);

        ActiveInGameMenu();

        CameraManager.instance.OnGameStart();
    }

    //메인메뉴 활성화
    private void ActiveMainMenu()
    {
        mainMenuDeco.SetActive(!mainMenuDeco.activeSelf);
        mainMenuUI.SetActive(!mainMenuUI.activeSelf);

        //CameraManager.instance.ChangeMainMenuCameraLive();
    }

    //인게임 플레이 활성화
    private void ActiveInGameMenu()
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);

        ActiveInGameIsland();
    }

    //인게임 데코용 섬 오브젝트 활성화
    private void ActiveInGameIsland()
    {
        if (!enableDecoIslands)
            return;

        foreach (GameObject island in inGameDecoIslands)
        {
            island.SetActive(!island.activeSelf);
        }
    }

    public void ActiveStageSelectMenu()
    {
        StageSelectMenu.SetCollectStarTextAction();

        stageSelectMenuObject.SetActive(!stageSelectMenuObject.activeSelf);
        mainMenuUIBundle.SetActive(!mainMenuUIBundle.activeSelf);
    }

    public void OnClickStageSelectNodeBtn(int stageNum, StageSelectNode selectedNode)
    {

        selectedStageNum = stageNum;

        selectedStageNode?.UnSelectEffect();

        selectedStageNode = selectedNode;
    }

    //스테이지 선택 메뉴에서 스테이지 노드 클릭시
    public void OnClickStageSelectStartBtn()
    {
        StartStageAsNumber(selectedStageNum);

        ActiveMainMenu();
        ActiveInGameMenu();
    }

    public void OnClickPlayBtn()
    {
        StartStageAsNumber(selectedStageNum);

        ActiveMainMenu();
        ActiveInGameMenu();

        DataManager.SaveData<int>("currentStage", selectedStageNum);

        CameraManager.instance.OnGameStart();
    }

    public void OnClickHomeBtn()
    {
        if (stageRestartCoroutin != null)
            StopCoroutine(stageRestartCoroutin);

        RestartCurrentStage();

        ActiveMainMenu();

        ActiveInGameMenu();

        CameraManager.instance.OnGoToMainMenu();

        currentStage.gameObject.SetActive(false);

        StageSelectMenu.SetCollectStarTextAction();
    }

    [ContextMenu("Get Stages In Childs")]
    void GetStagesInChilds()
    {
        var stages = playableStageParent.GetComponentsInChildren<Stage>(true);

        //stageList = new Stage[stages.Length];

        stageList = stages;

        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].name = "Stage " + (i + 1).ToString();

            if (stages[i].gameObject.GetComponentInChildren<Fan>() != null)
            {
                stages[i].name = stages[i].name + " + fan";
            }

            if (stages[i].gameObject.GetComponentInChildren<Gunner>() != null)
            {
                stages[i].name = stages[i].name + " + gunner";
            }

            if (stages[i].gameObject.GetComponentInChildren<JumpPanel>() != null)
            {
                stages[i].name = stages[i].name + " + jump";
            }

            if (stages[i].gameObject.GetComponentInChildren<Spike>() != null)
            {
                stages[i].name = stages[i].name + " + spike";
            }
        }

        for (int i = 0; i < 4; i++)
        {
            stageList[i].stageData.noAdStage = true;
        }
    }

    private void LoadCurrentStage()
    {
        currentStage.LoadStage();
        ActiveTutorial();

    }

    public void ActiveTutorial()
    {

        if (currentStage.stageData.tutorialStage)
            tutorialPanel.SetActive(true);
        else
            tutorialPanel.SetActive(false);
    }

    public void HideTutorial()
    {
        if (tutorialPanel.activeSelf)
            tutorialPanel.SetActive(false);
    }

    public void CallIR()
    {
        if (!currentStage.stageData.tutorialStage && !currentStage.stageData.noAdStage)
            AdsManager.ShowInterstitial();
    }

    /// <summary>
    /// 모든 스테이지가 언락되어 있는지 확인
    /// </summary>
    public bool CheckAllStageOpened(out int notOpenStageNumber)
    {
        for (int i = 0; i < stageList.Length; i++)
        {
            if (!stageList[i].stageData.openedStage)
            {
                notOpenStageNumber = i;
                return false;
            }
        }

        notOpenStageNumber = 0;
        return true;
    }

    /// <summary>
    /// 모든 스테이지가 클리어인지 확인
    /// </summary>
    public bool CheckAllStageClear(out int notclearStageNum)
    {
        for (int i = 0; i < stageList.Length; i++)
        {
            if (!stageList[i].stageData.clearStage)
            {
                notclearStageNum = i;
                return false;
            }
        }
        notclearStageNum = 0;
        return true;
    }

    void RandomStage()
    {
        randomStage = true;
        currentStageNumber = GetRandomStageNum();

        DataManager.SaveData<int>("currentStage", currentStageNumber);

        stageText.text = "STAGE " + (randomStageNum + 1);
        DataManager.SaveData<bool>("randomStage", true);
    }

    /// <summary>
    /// 1스테이지를 제외한 열려있고, 클리어한 스테이지 번호 랜덤으로 가져오기
    /// </summary>
    public int GetRandomStageNum()
    {
        for (int i = 0; i < 100; i++)
        {
            int randomNum = Random.Range(1, stageList.Length);
            print("현재 스테이지 : " + currentStageNumber + " 랜덤 스테이지 : " + randomNum);
            if (stageList[randomNum].stageData.clearStage && stageList[randomNum].stageData.openedStage && randomNum != currentStageNumber)
                return randomNum;
        }

        return 1;
    }
}
