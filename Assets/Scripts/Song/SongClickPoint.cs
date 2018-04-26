using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 記錄點擊物資料
// 非 ScriptableObject 須掛上 System.Serializable 才可以被記錄下來
// 一個 ScriptableObject 需要一個檔案來保存，但點擊物太多太小，用檔案不易管理
[System.Serializable]
public struct SongClickPoint
{
    // 點擊物種類，目前僅實作單次點擊，也只能側錄單次點擊
    public enum Type
    {
        Single = 0,
        Drag = 1,
        Sub = 2, 
    }

    // 種類
    public Type ClickType;
    // 時間點
    public float TimePoint;
    // 位置；y 雖然也被記錄下來，但不會被使用
    public Vector2 Position;
    // 目前沒用上的額外參數
    public float ExtraParameter;
}
