using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AnimationState
{
    DISAPPER_STATE = 0,
    SHOW_STATE,
    FIRST5_STATE,
    ANIM_STATE,
    END_STATE,
    OTHEROFFSET_STATE,              // 要跑去看另外一片
}

public class ScanAreaAnimation : MonoBehaviour
{

    // UI 相關
    public GameObject AreaImage;
    public Transform ParentNode;

    public Vector3 StartLocation = new Vector3(-40, 0, 0);

    // 動畫相關
    public AnimationState state = AnimationState.DISAPPER_STATE;
    public float ShowTime;
    public float First5Time;
    public float AnimTime;
    public float EndTime;
    public float OtherOffsetTime;
    public Color c;
    private float counterT = 0;

    // 陣列相關
    private List<GameObject> AreaImageArray = new List<GameObject>();
    private int AreaSize = 100;
    private int OffsetValue = 60;

    private void Start ()
    {
		// 生成 AreaSize x AreaSize 個
        for(int w = 0; w < AreaSize; w++)
        {
            // 生層物件
            GameObject tempObj = GameObject.Instantiate<GameObject>(AreaImage);
            tempObj.name = "Cover " + w.ToString();
            tempObj.transform.SetParent(ParentNode);
            tempObj.SetActive(true);

            RectTransform rectT = tempObj.GetComponent<RectTransform>();

            if(w > AreaSize / 2)
                rectT.anchoredPosition = new Vector3(w - AreaSize, 0, 0) + StartLocation;
            else
                rectT.anchoredPosition = new Vector3(w, 0, 0) + StartLocation;

            rectT.sizeDelta = new Vector2(1, 0);
            rectT.localRotation = Quaternion.identity;
            rectT.localScale = new Vector3(1, 1, 1);

            Image img = tempObj.GetComponent<Image>();
            img.color = c;

            AreaImageArray.Add(tempObj);
        }
	}

    private void Update()
    {
        counterT += Time.deltaTime;
        if (counterT >= ShowTime)
            state = AnimationState.SHOW_STATE;
        if (counterT >= First5Time)
            state = AnimationState.FIRST5_STATE;
        if (counterT >= AnimTime)
            state = AnimationState.ANIM_STATE;
        if (counterT >= EndTime)
        {
            state = AnimationState.END_STATE;

            for (int i = 0; i < AreaImageArray.Count; i++)
                AreaImageArray[i].GetComponent<RectTransform>().sizeDelta = new Vector2(1, AreaSize);
        }
        if (counterT >= OtherOffsetTime)
        {
            state = AnimationState.OTHEROFFSET_STATE;

            for (int w = 0; w < AreaSize; w++)
            {
                RectTransform rectT = AreaImageArray[w].GetComponent<RectTransform>();

                if (w > AreaSize / 2)
                    rectT.anchoredPosition = new Vector3(w - AreaSize, 0, 0) + StartLocation + new Vector3(OffsetValue, 0, 0);
                else
                    rectT.anchoredPosition = new Vector3(w, 0, 0) + StartLocation + new Vector3(OffsetValue, 0, 0);
            }
            this.enabled = false;
        }

        switch (state)
        {
            case AnimationState.SHOW_STATE:
                AreaImageArray[0].GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
                for(int i = 1; i < AreaImageArray.Count; i++)
                    AreaImageArray[i].GetComponent<RectTransform>().sizeDelta = new Vector2(1, 0);
                break;


            case AnimationState.FIRST5_STATE:
                float Percentage = (AnimTime - First5Time) / AreaSize / 5;
                int currentIndex = (int)((counterT - First5Time) / Percentage);

                // 開始找
                for (int i = 0; i < 5; i++)
                {
                    RectTransform rectTemp = AreaImageArray[i].GetComponent<RectTransform>();

                    if (i * AreaSize < currentIndex - currentIndex % AreaSize)
                        rectTemp.sizeDelta = new Vector2(1, AreaSize);
                    else if (i * AreaSize >= currentIndex)
                        rectTemp.sizeDelta = new Vector2(1, 0);
                    else
                    {
                        currentIndex %= AreaSize;
                        rectTemp.sizeDelta = new Vector2(1, currentIndex);
                    }
                }
                break;

            case AnimationState.ANIM_STATE:
                Percentage = (EndTime - AnimTime) / (AreaSize - 5) / AreaSize;
                currentIndex = (int)((counterT - AnimTime) / Percentage);

                // 開始找
                for (int i = 0; i < 5; i++)
                    AreaImageArray[i].GetComponent<RectTransform>().sizeDelta = new Vector2(1, AreaSize);
                for (int i = 5; i < AreaSize; i++)
                {
                    RectTransform rectTemp = AreaImageArray[i].GetComponent<RectTransform>();

                    if ((i - 5) * AreaSize < currentIndex - currentIndex % AreaSize)
                        rectTemp.sizeDelta = new Vector2(1, AreaSize);
                    else if ((i - 5) * AreaSize >= currentIndex)
                        rectTemp.sizeDelta = new Vector2(1, 0);
                    else
                    {
                        currentIndex %= AreaSize;
                        rectTemp.sizeDelta = new Vector2(1, currentIndex);
                    }
                }
                break;
        }
    }
}
