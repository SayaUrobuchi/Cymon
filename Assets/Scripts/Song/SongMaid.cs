using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 歌曲的「身體」，放入 SongData 作為靈魂後才真正完整
// StageMaid 只與 SongMaid 溝通，並不直接存取 SongData
public class SongMaid : MonoBehaviour
{
    // 靈魂的容器
    public SongData Data;

    // 以下是從靈魂取得一切用的轉介 property
    public AudioClip SongRef
    {
        get
        {
            return Data.SongRef;
        }
    }

    public string MainTitle
    {
        get
        {
            return Data.MainTitle;
        }
    }

    public string SubTitle
    {
        get
        {
            return Data.SubTitle;
        }
    }

    public float TempoLength
    {
        get
        {
            return Data.TempoLength;
        }
    }

    public List<SongClickPoint> ClickPoints
    {
        get
        {
            return Data.ClickPoints;
        }
    }

    // 讓 StageMaid 把側錄模式下錄到的資料儲存進來時用
    public void AddRecordToData(List<SongClickPoint> list)
    {
        Data.ClickPoints.AddRange(list);
        // 以下語法是將 asset 檔案的變更真正儲存到硬碟，僅限編輯時用
        // 不加 #if 判斷的話，打包時會 compile error
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
