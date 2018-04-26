using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageMaid : MonoBehaviour
{
    public enum State
    {
        None, 
        Song, 
        Result, 
    }

    public enum Grade
    {
        Perfect, 
        Great, 
        Bad, 
        Miss, 
    }

    private static StageMaid self;
    public static StageMaid Summon
    {
        get
        {
            return self;
        }
    }

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

    [Header("Template")]
    public ClickPointMaid SingleClickTemplate;
    public Text PerfectTextTemplate;
    public Text GreatTextTemplate;
    public Text BadTextTemplate;
    public Text MissTextTemplate;

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

    [Header("UIConfig")]
    public float ComboTextShakeTime = .1f;
    public float ComboTextShakePower = 2f;

    [Header("TempoBar")]
    public RectTransform TempoBarRef;
    public float TempoBarRange = 50f;

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

    public float SongTimer
    {
        get
        {
            return songTimer;
        }
    }

	// Use this for initialization
	void Start ()
    {
        self = this;
        state = State.None;
	}

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
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
                ReadyText.gameObject.SetActive(true);
                Color c = ReadyText.color;
                c.a = 1f;
                ReadyText.color = c;
                ready = false;
                state = State.Song;
            }
            break;
        case State.Song:
            {
                songTimer += Time.deltaTime;

                UpdateReadyText(songTimer);
                UpdateCountdownText(songTimer);
                
                UpdateTempoBar(songTimer);
                UpdateClickPoints(songTimer);

                if (Input.anyKeyDown)
                {
                    Debug.Log(songTimer);
                }
            }
            break;
        }
	}

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
                SongPlayer.Play();
                ready = true;
            }
        }
    }

    private void UpdateTopPanel()
    {
        MainTitleText.text = SongMaidRef.MainTitle;
        SubTitleText.text = SongMaidRef.SubTitle;
        UpdateScoreText(score);
        UpdateComboText(combo);
    }

    private void UpdateScoreText(int sc)
    {
        ScoreText.text = sc.ToString();
    }

    private void UpdateComboText(int comb, bool changeEffect = false)
    {
        ComboText.text = comb.ToString();
        if (changeEffect)
        {
            ComboText.GetComponent<ShakeMaid>().Shake(ComboTextShakeTime, ComboTextShakePower, true);
        }
    }

    private void UpdateCountdownText(float countdown)
    {
        CountDownText.text = countdown.ToString("0.00");
    }

    private void UpdateTempoBar(float timer)
    {
        TempoBarRef.anchoredPosition = new Vector2(0f, GetTempoBarPositionByTime(timer));
    }

    public float GetCurrentTempoBarPosition()
    {
        return GetTempoBarPositionByTime(songTimer);
    }

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

    private void UpdateClickPoints(float timer)
    {
        while (nextIdx < nextPoints.Count)
        {
            if (songTimer + TimeForAppear > nextPoints[nextIdx].TimePoint)
            {
                ClickPointMaid maid = Instantiate(SingleClickTemplate);
                maid.transform.SetParent(ClickPointContainer);
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

        for (int i = currentIdx; i < currentPoints.Count; i++)
        {
            if (!currentPoints[i].IsFinished(this))
            {
                currentPoints[i].UpdateState(this);
            }
        }
        for (; currentIdx < currentPoints.Count; currentIdx++)
        {
            if (!currentPoints[currentIdx].IsFinished(this))
            {
                break;
            }
            Destroy(currentPoints[currentIdx].gameObject);
        }
    }

    public void ClickOnPoints(ClickPointMaid maid)
    {
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

    public void ClickMiss(ClickPointMaid maid)
    {
        OnClick(Grade.Miss);
        maid.GenerateResultText(MissTextTemplate);
    }

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

    public void RecordClick(Vector2 pos)
    {
        SongClickPoint rec = new SongClickPoint();
        rec.TimePoint = songTimer;
        rec.Position = pos;
        recordList.Add(rec);
    }

    [ContextMenu("Record")]
    public void RecordToSongData()
    {
        if (SongMaidRef != null)
        {
            SongMaidRef.AddRecordToData(recordList);
        }
    }

    [ContextMenu("ClearRecord")]
    public void ClearRecord()
    {
        recordList.Clear();
    }
}
