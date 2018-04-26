using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickRecordMaid : MonoBehaviour
{
    private RectTransform rectTransform;

    // Use this for initialization
    void Start ()
    {
		rectTransform = transform as RectTransform;
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void ClickRecord()
    {
        Vector2 localpoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out localpoint);

        Vector2 normalizedPoint = Rect.PointToNormalized(rectTransform.rect, localpoint);
        StageMaid.Summon.RecordClick(normalizedPoint);
    }
}
