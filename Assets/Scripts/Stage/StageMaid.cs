using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 管理關卡一切的女僕長
public class StageMaid : MonoBehaviour
{
    // 當前狀態的種類
    public enum State
    {
        None, 
        Song, 
        Result, 
    }

    // 點擊評分種類
    public enum Grade
    {
        Perfect, 
        Great, 
        Bad, 
        Miss, 
    }

    // Singleton Pattern，方便大家用 static 存取到這關卡唯一存在的女僕長，無須 reference
    private static StageMaid self;
    public static StageMaid Summon
    {
        get
        {
            return self;
        }
    }

    // 各種東西的 reference
    [Header("Reference")]
    public SongMaid SongMaidRef;
    public AudioSource SongPlayer;
    public Text CountDownText;
    public Text ReadyText;
    public RectTransform ClickPointContainer;
    public Text ScoreText;
    public Text ComboText;
    public Text MainTitleText;
    public Text SubTitleText;

    // 各種點擊物或點擊結果字樣等樣板
    [Header("Template")]
    public ClickPointMaid SingleClickTemplate;
    public Text PerfectTextTemplate;
    public Text GreatTextTemplate;
    public Text BadTextTemplate;
    public Text MissTextTemplate;

    // 與歌曲相關的參數，如點擊物出現消失時間長短、點擊評價判定和得分等
    [Header("SongConfig")]
    public float ReadyCountdown = 2f;
    public float TimeForAppear = 2f;
    public float TimeForStay = 2f;
    public float TimeForDisappear = 1f;
    public float PerfectJudge = .2f;
    public float GreatJudge = 1f;
    public float BadJudge = 2f;
    public int PerfectScore = 40;
    public int GreatScore = 20;
    public int BadScore = 10;
    public int MissScore = 0;

    // UI 相關參數
    [Header("UIConfig")]
    public float ComboTextShakeTime = .1f;
    public float ComboTextShakePower = 2f;

    // 橫槓相關
    [Header("TempoBar")]
    public RectTransform TempoBarRef;
    public float TempoBarRange = 50f;

    // 編輯器限定功能
    [Header("EditorOnly")]
    public bool RecordMode = false;
    public float StartFromTime = 0f;
    
    private State state;
    private float songTimer = 0f;
    private bool ready;
    private List<SongClickPoint> nextPoints;
    private List<ClickPointMaid> currentPoints;
    private int nextIdx;
    private int currentIdx;
    private int score;
    private int combo;
    private int perfectCount;
    private int greatCount;
    private int badCount;
    private int missCount;

    private List<SongClickPoint> recordList = new List<SongClickPoint>();

    // 獲取歌曲目前時間
    public float SongTimer
    {
        get
        {
            return songTimer;
        }
    }

    // 初始化
	void Start ()
    {
        self = this;
        state = State.None;
	}

    // 每次更新
    void Update()
    {
        switch (state)
        {
        // 初始化
        case State.None:
            {
                SongPlayer.clip = SongMaidRef.SongRef;
                nextIdx = 0;
                currentIdx = 0;
                nextPoints = SongMaidRef.ClickPoints;
                currentPoints = new List<ClickPointMaid>();
                score = 0;
                combo = 0;
                perfectCount = 0;
                greatCount = 0;
                badCount = 0;
                missCount = 0;
                UpdateTopPanel();
                songTimer = -ReadyCountdown;
                // 非編輯模式下應該一律從頭播放
#if UNITY_EDITOR
                SongPlayer.time = StartFromTime;
#else
                SongPlayer.time = 0f;
#endif
                SongPlayer.PlayDelayed(ReadyCountdown);
                ReadyText.gameObject.SetActive(true);
                Color c = ReadyText.color;
                c.a = 1f;
                ReadyText.color = c;
                ready = false;
                state = State.Song;
            }
            break;
        // 從倒計時（READY）到歌結束（其實目前結束後什麼事也不會發生）
        case State.Song:
            {
                // 計時
                songTimer += Time.deltaTime;

                // 更新 UI
                UpdateReadyText(songTimer);
                UpdateCountdownText(songTimer);
                
                // 更新橫槓和點擊物
                UpdateTempoBar(songTimer);
                UpdateClickPoints(songTimer);
            }
            break;
        }
	}

    // 更新 READY 字樣
    private void UpdateReadyText(float timer)
    {
        if (!ready)
        {
            Color c = ReadyText.color;
            if (timer < 0f)
            {
                float alpha = -timer / ReadyCountdown;
                if (alpha > 0.5f)
                {
                    alpha = 1f;
                }
                else
                {
                    alpha *= 2;
                }
                c.a = alpha;
                ReadyText.color = c;
            }
            else
            {
                ReadyText.gameObject.SetActive(false);
                ready = true;
            }
        }
    }

    // 更新 UI 面板
    private void UpdateTopPanel()
    {
        MainTitleText.text = SongMaidRef.MainTitle;
        SubTitleText.text = SongMaidRef.SubTitle;
        UpdateScoreText(score);
        UpdateComboText(combo);
    }

    // 更新分數
    private void UpdateScoreText(int sc)
    {
        ScoreText.text = sc.ToString();
    }

    // 更新 combo （可附帶振動）
    private void UpdateComboText(int comb, bool changeEffect = false)
    {
        ComboText.text = comb.ToString();
        if (changeEffect)
        {
            ComboText.GetComponent<ShakeMaid>().Shake(ComboTextShakeTime, ComboTextShakePower, true);
        }
    }

    // 更新計時
    private void UpdateCountdownText(float countdown)
    {
        CountDownText.text = countdown.ToString("0.00");
    }

    // 更新橫槓
    private void UpdateTempoBar(float timer)
    {
        TempoBarRef.anchoredPosition = new Vector2(0f, GetTempoBarPositionByTime(timer));
    }

    // 計算橫槓現在位置
    public float GetCurrentTempoBarPosition()
    {
        return GetTempoBarPositionByTime(songTimer);
    }

    // 計算特定時間點橫槓位置
    public float GetTempoBarPositionByTime(float time)
    {
        float progress = time / SongMaidRef.TempoLength;
        progress -= Mathf.Floor(progress);
        if (progress > 0.5f)
        {
            progress = 1 - progress;
        }
        progress *= 2;
        return (-1f + progress * 2) * TempoBarRange;
    }

    // 更新點擊物們
    private void UpdateClickPoints(float timer)
    {
        // 側錄模式不生成點擊物
        if (RecordMode)
        {
            return;
        }
        // 搜尋是否有點擊物該出現了，若有就加到更新列表中
        while (nextIdx < nextPoints.Count)
        {
            if (songTimer + TimeForAppear > nextPoints[nextIdx].TimePoint)
            {
                // 生成點擊物
                ClickPointMaid maid = Instantiate(SingleClickTemplate);
                // 放到特定的 UI 子世界去
                maid.transform.SetParent(ClickPointContainer);
                // 後出現的應該出現在下層，避免擋住先出現的點擊物；先出現應該先被點擊
                maid.transform.SetAsFirstSibling();
                maid.Init(nextPoints[nextIdx]);
                currentPoints.Add(maid);
                nextIdx++;
            }
            else
            {
                break;
            }
        }

        // 更新列表中的所有點擊物
        for (int i = currentIdx; i < currentPoints.Count; i++)
        {
            if (!currentPoints[i].IsFinished(this))
            {
                currentPoints[i].UpdateState(this);
            }
        }
        // 排除所有已結束的點擊物
        for (; currentIdx < currentPoints.Count; currentIdx++)
        {
            if (!currentPoints[currentIdx].IsFinished(this))
            {
                break;
            }
            Destroy(currentPoints[currentIdx].gameObject);
        }
    }

    // 點擊物被回報點擊時觸發的事件
    public void ClickOnPoints(ClickPointMaid maid)
    {
        // 點擊評價判定
        float judge = Mathf.Abs(songTimer - maid.Data.TimePoint);
        if (judge < PerfectJudge)
        {
            OnClick(Grade.Perfect);
            maid.GenerateResultText(PerfectTextTemplate);
        }
        else if (judge < GreatJudge)
        {
            OnClick(Grade.Great);
            maid.GenerateResultText(GreatTextTemplate);
        }
        else if (judge < BadJudge)
        {
            OnClick(Grade.Bad);
            maid.GenerateResultText(BadTextTemplate);
        }
        else
        {
            OnClick(Grade.Miss);
            maid.GenerateResultText(MissTextTemplate);
        }
    }

    // 必定 miss 的點擊
    public void ClickMiss(ClickPointMaid maid)
    {
        OnClick(Grade.Miss);
        maid.GenerateResultText(MissTextTemplate);
    }

    // 點擊時因應評價發生的影響，各評價統計次數、分數的變更、combo 存續等
    private void OnClick(Grade g)
    {
        switch (g)
        {
        case Grade.Perfect:
            perfectCount++;
            combo++;
            break;
        case Grade.Great:
            greatCount++;
            combo++;
            break;
        case Grade.Bad:
            badCount++;
            break;
        case Grade.Miss:
            missCount++;
            combo = 0;
            break;
        }
        GainScore(g);
        UpdateComboText(combo, g!=Grade.Bad);
    }

    // 依評價獲得分數，按 combo 等比成長
    private void GainScore(Grade g)
    {
        float rate = Mathf.Pow(1.01f, combo);
        int sc = 0;
        switch (g)
        {
        case Grade.Perfect:
            sc = PerfectScore;
            break;
        case Grade.Great:
            sc = GreatScore;
            break;
        case Grade.Bad:
            sc = BadScore;
            break;
        case Grade.Miss:
            sc = MissScore;
            break;
        }
        score += Mathf.RoundToInt(sc * rate);
        UpdateScoreText(score);
    }

    // 側錄模式下記錄點擊；記錄的點擊其實也能錄成 replay 重現
    public void RecordClick(Vector2 pos)
    {
        SongClickPoint rec = new SongClickPoint();
        rec.TimePoint = songTimer;
        rec.Position = pos;
        recordList.Add(rec);
    }

    // 在 component 右鍵選單加上 Record 一項，儲存目前側錄的點擊用，不執行就不會真的被儲存
    [ContextMenu("Record")]
    public void RecordToSongData()
    {
        if (SongMaidRef != null)
        {
            SongMaidRef.AddRecordToData(recordList);
        }
    }

    // 同上，功用是清掉目前已側錄的點擊
    [ContextMenu("ClearRecord")]
    public void ClearRecord()
    {
        recordList.Clear();
    }
}
