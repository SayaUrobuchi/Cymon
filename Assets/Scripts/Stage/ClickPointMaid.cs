using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickPointMaid : MonoBehaviour
{
    public SongClickPoint Data;

    private bool clicked = false;
    private bool finished = false;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private float clickTime;

    public void Init(SongClickPoint data)
    {
        Data = data;
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = transform as RectTransform;
        Relocate();
        gameObject.SetActive(true);
    }

    private void Relocate()
    {
        rectTransform.anchoredPosition = new Vector2((Data.Position.x - 0.5f) * (transform.parent as RectTransform).rect.width, 
                                                        StageMaid.Summon.GetTempoBarPositionByTime(Data.TimePoint));
    }

    public bool IsFinished(StageMaid maid)
    {
        return finished;
    }

    public void UpdateState(StageMaid maid)
    {
        float progress;
        if (!clicked)
        {
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
            progress = Mathf.Min(1f, (StageMaid.Summon.SongTimer - clickTime) / maid.TimeForDisappear);
            UpdateDisappear(progress);
            if (progress >= 1f)
            {
                finished = true;
            }
        }
    }

    private void UpdateAppear(float progress)
    {
        canvasGroup.alpha = 1f - progress;
        float scale = 0.7f + 0.3f * (1f - progress);
        rectTransform.localScale = new Vector3(scale, scale, 1f);
    }

    private void UpdateStay(float progress)
    {
        canvasGroup.alpha = 1;
        rectTransform.localScale = Vector3.one;
    }

    private void UpdateDisappear(float progress)
    {
        canvasGroup.alpha = progress;
        float scale = 1f + 0.5f * progress;
        rectTransform.localScale = new Vector3(scale, scale, 1f);
    }

    private void DisableClick()
    {
        GetComponent<Image>().raycastTarget = false;
    }

    public void GenerateResultText(Text resultTemplate)
    {
        Text item = Instantiate(resultTemplate);
        item.gameObject.SetActive(true);
        item.rectTransform.SetParent(rectTransform, false);
        DisableClick();
    }

    public void Click()
    {
        if (!clicked)
        {
            clicked = true;
            UpdateStay(0f);
            clickTime = StageMaid.Summon.SongTimer;
            StageMaid.Summon.ClickOnPoints(this);
        }
    }
}
