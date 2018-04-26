using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 在譜面編輯模式中，負責記錄點擊時間點和位置，回報給 StageMaid
public class ClickRecordMaid : MonoBehaviour
{
    // 過程需要把點擊位置從「畫面上的某一點」換算回「UI 的座標系」，這時會用到
    private RectTransform rectTransform;

    // 初始化
    void Start ()
    {
		rectTransform = transform as RectTransform;
	}

    // 連接到點擊側錄用物件，作為被點擊時觸發的事件
    public void ClickRecord()
    {
        if (StageMaid.Summon.RecordMode)
        {
            // 生成點擊物時是在 UI 座標系上，而滑鼠點擊位置是在螢幕座標系上，需要換算
            // 將「螢幕座標系」換算成「UI座標系」
            Vector2 localpoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out localpoint);
            // 將「UI座標系」從絕對座標經過「一般化」換算回相對座標
            // 一般化之後會是以 0~1 記錄位置，(0, 0) 左上、(1, 1) 右下
            // 這樣就算整個座標系縮放了、UI 大小變了，比例也能維持不變
            Vector2 normalizedPoint = Rect.PointToNormalized(rectTransform.rect, localpoint);
            // 將換算後的結果傳給 StageMaid 作記錄
            StageMaid.Summon.RecordClick(normalizedPoint);
        }
    }
}
