using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    //public int stageNumber = 0;
    public bool openedStage = false;
    [field: SerializeField] public bool clearStage { get; set; } = false;
    [field: SerializeField] public int score { get; set; }

    [field: SerializeField] public bool tutorialStage { get; private set; }
    [field: SerializeField] public bool noAdStage { get; set; } = false;


}
