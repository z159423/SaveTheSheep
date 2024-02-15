using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectNode : MonoBehaviour
{
    private Stage stage;

    [SerializeField] private int thisStageNum;

    [SerializeField] public Button btn;
    [SerializeField] private GameObject[] clearStars;
    [SerializeField] private Text stageNumText;
    [SerializeField] private GameObject lockCover;
    [SerializeField] private GameObject selectOutline;


    public void StageSelectNodeInit(int stageNum, Stage stage)
    {
        this.stage = stage;
        thisStageNum = stageNum;

        //btn.onClick.AddListener(delegate { StageManager.instance.OnClickStageSelectNodeBtn(stageNum); });

        stageNumText.text = (stageNum + 1).ToString();

        InitThisNode();
    }

    /// <summary>
    /// 해당 스테이지 노드를 초기화 하는 함수입니다.
    /// 스테이지가 열려있는지, 스테이지의 스코어를 UI에 표시해 줍니다.
    /// </summary>
    public void InitThisNode()
    {

        if (stage.stageData.openedStage)
        {
            lockCover.SetActive(false);
            btn.interactable = true;
        }
        else
        {
            lockCover.SetActive(true);
            btn.interactable = false;
        }

        //스테이지 클리어 별 개수 가져와서 채워넣기

        for (int i = 0; i < stage.stageData.score; i++)
        {
            clearStars[i].SetActive(true);
        }
    }

    /// <summary>
    /// 스테이지 노드를 선택하였을때 기능을 수행하는 함수입니다.
    /// </summary>
    public void SelectThisNodeEffect()
    {
        print(thisStageNum + " 스테이지가 선택되었습니다.");

        StageManager.instance.OnClickStageSelectNodeBtn(thisStageNum, this);

        selectOutline.SetActive(true);
    }

    public void UnSelectEffect()
    {
        selectOutline.SetActive(false);
    }
}
