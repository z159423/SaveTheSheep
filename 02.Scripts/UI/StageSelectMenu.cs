using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StageSelectMenu : MonoBehaviour
{

    [SerializeField] private int selectNodeCount = 12;

    [SerializeField] private List<StageSelectNode> generatedNode = new List<StageSelectNode>();

    [SerializeField] private StageManager stageManager;
    [SerializeField] private Transform stageNodeContenctParent;
    [SerializeField] private GameObject stageSelectNodePrefab;
    [SerializeField] private GameObject stageSelectPanelPrefab;
    [SerializeField] private GameObject panelCircleNode;
    [SerializeField] private Transform panelCircleTransform;
    [SerializeField] private Text collectedStarText;

    [Space]

    [SerializeField] private SwipeUI swipeUI;

    public static Action SetCollectStarTextAction;

    private void Awake()
    {
        SetCollectStarTextAction = () => { SetCollectedStarText(); };
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadStageData());

        IEnumerator LoadStageData()
        {
            {
                yield return new WaitForSeconds(2f);

            }
        }

        GenerateStageSelectNodes();

        SetCollectedStarText();
    }

    //스테이지 선택 메뉴의 스테이지 노드들 생성
    private void GenerateStageSelectNodes()
    {
        StartCoroutine(generateStageNode());

        IEnumerator generateStageNode()
        {
            int panelAmount = ((stageManager.stageList.Length - 1) / (selectNodeCount)) + 1;

            print("페이지 개수 : " + panelAmount);

            //노드가 12개 들어가는 판넬 추가
            for (int i = 0; i < panelAmount; i++)
            {
                var panel = Instantiate(stageSelectPanelPrefab, stageNodeContenctParent);

                int nodeCount = Mathf.Clamp(stageManager.stageList.Length - (i * selectNodeCount), 0, selectNodeCount);
                for (int j = 0; j < nodeCount; j++)
                {
                    var gameObject = Instantiate(stageSelectNodePrefab, panel.transform);

                    var node = gameObject.GetComponent<StageSelectNode>();

                    node.StageSelectNodeInit((i * selectNodeCount) + j, stageManager.stageList[(i * selectNodeCount) + j]);

                    generatedNode.Add(node);
                    yield return null;
                }

                //노드 개수가 한 페이지 개수보다 작으면 페이지에 노드가 비어보이지 않게 남은 칸은 잠겨있는 노드로 채워줌
                if (nodeCount < selectNodeCount)
                {
                    int remainderNodeCount = selectNodeCount - nodeCount;

                    for (int j = 0; j < remainderNodeCount; j++)
                    {
                        var gameObject = Instantiate(stageSelectNodePrefab, panel.transform);
                    }
                }

                var circleNode = Instantiate(panelCircleNode, panelCircleTransform);

                swipeUI.circleContents.Add(circleNode.transform.GetChild(0).gameObject);
            }

            swipeUI.InitPages();
            //swipeUI.SetScrollBarValue(panelAmount - 1);

            swipeUI.selectMenuGenerateComplite = true;

            // for (int i = 1; i < stageManager.stageList.Length; i++)
            // {
            //     var gameObject = Instantiate(stageSelectNodePrefab, stageNodeContenctParent);

            //     var node = gameObject.GetComponent<StageSelectNode>();

            //     node.StageSelectNodeInit(i, stageManager.stageList[i]);

            //     generatedNode.Add(node);
            //     yield return null;
            // }

            //진행중이던 마지막 스테이지 자동으로 선택
            SelectNode(DataManager.LoadData<int>("currentStage"));
        }
    }

    public void ReinitNode(int stageNum)
    {
        generatedNode[stageNum].InitThisNode();
    }

    public void SelectNode(int stageNum)
    {
        if (0 <= generatedNode.Count && stageNum < generatedNode.Count)
            generatedNode[stageNum].SelectThisNodeEffect();
        else
            generatedNode[0].SelectThisNodeEffect();
    }

    public void ReInitAllGeneratedNode()
    {
        for (int i = 0; i < generatedNode.Count; i++)
        {
            generatedNode[i].InitThisNode();
        }
    }

    /// <summary>
    /// 수집한 별 개수 텍스트 수정
    /// </summary>
    private void SetCollectedStarText()
    {
        int collectedStar = 0;
        int collectableStar = stageManager.stageList.Length * 3;

        for (int i = 0; i < stageManager.stageList.Length; i++)
        {
            collectedStar += stageManager.stageList[i].stageData.score;
        }

        collectedStarText.text = collectedStar + " / " + collectableStar;
    }
}
