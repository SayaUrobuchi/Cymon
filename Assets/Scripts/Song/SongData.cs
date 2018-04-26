using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 儲存歌曲和譜面相關資訊，一首需要一個 .asset 檔案來儲存
// 可在功能表的 Create/Cymon/SongData 建立
public class SongData : ScriptableObject
{
    // 歌曲的檔案
    public AudioClip SongRef;
    // 主標題
    public string MainTitle = "";
    // 副標題
    public string SubTitle = "";
    // 橫槓一周回的時間，單位為秒
    public float TempoLength = 1f;
    // 點擊物們，List 同 C++ std::vector，是可變長度陣列
    public List<SongClickPoint> ClickPoints = new List<SongClickPoint>();
}
