using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{

    public static void SaveData<T>(string key, T data)
    {
        ES3.Save<T>(key, data);
    }

    public static T LoadData<T>(string key)
    {
        if (ES3.KeyExists(key))
            return ES3.Load<T>(key);
        else
        {
            Debug.LogWarning(key + " 에 해당하는 세이브 파일이 존재하지 않습니다.");
            return default(T);
        }
    }

    public static void SaveStageData(Stage[] stageList)
    {
        StageData[] stageData = new StageData[stageList.Length];

        for (int i = 0; i < stageData.Length; i++)
        {
            stageData[i] = stageList[i].stageData;
        }

        SaveData<StageData[]>("stageData", stageData);
    }

    public static void LoadStageData(ref Stage[] stages)
    {
        //스테이지 세이브 가져오기
        if (ES3.KeyExists("stageData"))
        {
            var stageData = DataManager.LoadData<StageData[]>("stageData");
            for (int i = 0; i < stageData.Length && i < StageManager.instance.stageList.Length; i++)
            {
                stages[i].stageData = stageData[i];
            }
        }
        else
        {
            Debug.LogWarning("스테이지 세이브 파일이 없습니다.");
        }

        //StageManager.instance.isLoadStageData = true;
    }
}
