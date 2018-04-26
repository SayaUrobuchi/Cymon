using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 管理個別點擊物的女僕，會持有一個點擊物資料 SongClickPoint
public class ClickPointMaid : MonoBehaviour
{
    // 靈魂的容器
    public SongClickPoint Data;

    // 被按過了沒
    private bool clicked = false;
    // 完全消失了沒；完全消失前都還需要被 update
    private bool finished = false;
    // 為了淡出淡入，須要 CanvasGroup 把底下子物件的 alpha 一起改掉（雖然目前沒有子物件）
    private CanvasGroup canvasGroup;
    // RectTransform 不時會用到所以先抓起來放
    private RectTransform rectTransform;
    // 記住被玩家點擊的時間點
    private float clickTime;

    // 初始化
    public void Init(SongClickPoint data)
    {
        Data = data;
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = transform as RectTransform;
        // 重新定位位置
        Relocate();
        // 預設是關掉的，不然樣板會在畫面上跑出來；所以生成後需要 active
        gameObject.SetActive(true);
    }

    // 重新定位
    private void Relocate()
    {
        // x 原定位是 0(左端點) 到 1(右端點)，但原點在正中央，所以轉成 -0.5 ~ +0.5
        // y 就抓確切時間點的橫槓位置，這個問 StageMaid
        rectTransform.anchoredPosition = new Vector2((Data.Position.x - 0.5f) * (transform.parent as RectTransform).rect.width, 
                                                        StageMaid.Summon.GetTempoBarPositionByTime(Data.TimePoint));
    }

    // 是否已消失、無須再 Update
    public bool IsFinished(StageMaid maid)
    {
        return finished;
    }

    // 其實就 Update，但是讓 StageMaid 集中管理，方便做暫停或變更時間經過的快慢等處理
    public void UpdateState(StageMaid maid)
    {
        // 每個狀態的進度，0 表示剛開始，1 表示已結束
        float progress;
        // 如果還沒被按過
        if (!clicked)
        {
            // 歌曲時間切成三段：確切時間點前（淡入）、確切時間點後到停留時間結束前（不變）、miss 判定時間後（自動點擊自己觸發 miss 判定）
            if (maid.SongTimer < Data.TimePoint)
            {
                progress = Mathf.Min(1f, 0.2f + (Data.TimePoint - maid.SongTimer) / maid.TimeForAppear);
                UpdateAppear(progress);
            }
            else if (maid.SongTimer < Data.TimePoint + maid.TimeForStay)
            {
                UpdateStay(0f);
            }
            else if (maid.SongTimer > Data.TimePoint + maid.BadJudge)
            {
                Click();
            }
        }
        else
        {
            // 點擊後進入消失過程，由點擊時間起算
            progress = Mathf.Min(1f, (StageMaid.Summon.SongTimer - clickTime) / maid.TimeForDisappear);
            UpdateDisappear(progress);
            // 消失過程結束後，設為無須再處理
            if (progress >= 1f)
            {
                finished = true;
            }
        }
    }

    // 處理出現時的淡入
    private void UpdateAppear(float progress)
    {
        canvasGroup.alpha = 1f - progress;
        float scale = 0.7f + 0.3f * (1f - progress);
        rectTransform.localScale = new Vector3(scale, scale, 1f);
    }

    // 處理停止不變時
    private void UpdateStay(float progress)
    {
        canvasGroup.alpha = 1;
        rectTransform.localScale = Vector3.one;
    }

    // 處理消失
    private void UpdateDisappear(float progress)
    {
        canvasGroup.alpha = progress;
        float scale = 1f + 0.5f * progress;
        rectTransform.localScale = new Vector3(scale, scale, 1f);
    }

    // 取消作為點擊目標，以免在點過後，仍然搶走點擊
    private void DisableClick()
    {
        GetComponent<Image>().raycastTarget = false;
    }

    // 生成結果字樣：Perfect, Great, Bad, Miss
    public void GenerateResultText(Text resultTemplate)
    {
        Text item = Instantiate(resultTemplate);
        item.gameObject.SetActive(true);
        item.rectTransform.SetParent(rectTransform, false);
        DisableClick();
    }

    // 點擊物被點擊時觸發的事件
    public void Click()
    {
        if (!clicked)
        {
            // 設為已點擊，狀態更改（可能在出現中就被點擊）
            clicked = true;
            UpdateStay(0f);
            // 記錄點擊時間
            clickTime = StageMaid.Summon.SongTimer;
            // 回報 StageMaid 點擊成功判定，分數與 combo 等細節由 StageMaid 管理，非點擊物的職責
            StageMaid.Summon.ClickOnPoints(this);
        }
    }
}
