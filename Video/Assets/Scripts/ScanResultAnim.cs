using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ScanAreaState
{
    DISAPPER_STATE = 0,
    BLENDING_IN_STATE,
    SHOW_STATE,
    FIRST5_STATE,
    ANIM_STATE,
    END_STATE
}

public class ScanResultAnim : MonoBehaviour
{
    [Header("===== 物件 =====")]
    public Sprite[] ImageList;
    public GameObject ScanGroup;
    public Transform ScanResult;
    private List<GameObject> ScanGroupList = new List<GameObject>();
    private List<Image> ScanBackgroundList = new List<Image>();
    private List<Image> ScanResultList = new List<Image>();
    public Vector3 StartPos = new Vector3(1525, 240);

    [Header("===== 時間 =====")]
    public float ShowTime;                          // 顯示的秒數
    public float First5Time;
    public float AnimTime;
    public float EndTime;

    [Header("===== 特效的時間 =====")]
    public float BlendingInTime;                    // 要淡進的時間 (所以顯示是 ShowTime - BlendingTime 要開始)
    public float MoveBackTime;                      // 顯示完一張要往後

    // 動畫
    public ScanAreaState state = ScanAreaState.DISAPPER_STATE;
    private float counterT = 0;
    private float MoveOffset = 10;
    private int AreaSize = 100;
    private float MovingStartTime;
    private int FladeOutStart = 5;
    private int FladeOutEnd = 10;


    private void Start()
    {
        for(int i = 0; i < AreaSize; i++)
        {
            GameObject tempObj = GameObject.Instantiate<GameObject>(ScanGroup);
            tempObj.name = "Scan Group " + i.ToString();
            tempObj.transform.SetParent(ScanResult);
            tempObj.SetActive(false);
            tempObj.transform.position = ScanGroup.transform.position;

            Image BackgroundImage = tempObj.GetComponent<Image>();
            BackgroundImage.color = new Color(1, 1, 1, 0);

            Image ScanImage = tempObj.GetComponentsInChildren<Transform>(true)[1].GetComponent<Image>();
            ScanImage.color = new Color(1, 1, 1, 0);
            ScanImage.sprite = ImageList[i % ImageList.Length];
            ScanImage.fillAmount = 0;

            ScanGroupList.Add(tempObj);
            ScanBackgroundList.Add(BackgroundImage);
            ScanResultList.Add(ScanImage);           
        }
    }

    private void Update()
    {
        counterT += Time.deltaTime;
        if (counterT >= ShowTime - BlendingInTime)
            state = ScanAreaState.BLENDING_IN_STATE;
        if (counterT >= ShowTime)
            state = ScanAreaState.SHOW_STATE;
        if (counterT >= First5Time)
            state = ScanAreaState.FIRST5_STATE;
        if (counterT >= AnimTime)
            state = ScanAreaState.ANIM_STATE;
        if (counterT >= EndTime)
            state = ScanAreaState.END_STATE;

        float Percentage;
        switch (state)
        {
            case ScanAreaState.BLENDING_IN_STATE:
                {
                    Percentage = (counterT - (ShowTime - BlendingInTime)) / BlendingInTime;
                    Percentage = Mathf.Clamp01(Percentage);

                    ScanGroupList[0].SetActive(true);
                    ScanBackgroundList[0].color = new Color(1, 1, 1, Percentage);
                    ScanResultList[0].color = new Color(1, 1, 1, Percentage);
                    ScanResultList[0].fillAmount = 0.2f;
                    break;
                }
            case ScanAreaState.SHOW_STATE:
                {
                    ScanGroupList[0].SetActive(true);
                    ScanBackgroundList[0].color = new Color(1, 1, 1, 1);
                    ScanResultList[0].color = new Color(1, 1, 1, 1);
                    ScanResultList[0].fillAmount = 0.2f;
                    break;
                }
            case ScanAreaState.FIRST5_STATE:
                {
                    // 總共幾%
                    Percentage = (counterT - First5Time) / (AnimTime - First5Time);

                    // 看在第幾張圖
                    int Part = (int)(Percentage * 5);

                    // 前半段
                    for(int i = 0; i < Part; i++)
                    {
                        ScanBackgroundList[i].fillAmount = 1;
                        ScanResultList[i].fillAmount = 1;
                    }


                    // 如果進到這段，代表有要考慮往後移 & 淡進的效果
                    if(Part > 0)
                    {
                        ScanGroupList[Part].SetActive(true);

                        // 後移 & 淡進的效果
                        float MoveBackTimePart = MoveBackTime / (AnimTime - First5Time);
                        if (Percentage - 0.2f * Part < MoveBackTimePart)
                        {
                            // 後移
                            for (int i = Part - 1; i >= 0; i--)
                                ScanGroupList[i].transform.position = StartPos +                // 開始位置
                                    ((Part - 1) - i) * new Vector3(MoveOffset, MoveOffset) +    // 每個的 Offset
                                    new Vector3(MoveOffset, MoveOffset) * (Percentage - 0.2f * Part) / MoveBackTimePart;    // 正在移動的動畫

                            // 淡進
                            ScanGroupList[Part].SetActive(true);
                            ScanBackgroundList[Part].color = new Color(1, 1, 1, (Percentage - 0.2f * Part) / MoveBackTimePart);
                            ScanResultList[Part].color = new Color(1, 1, 1, (Percentage - 0.2f * Part) / MoveBackTimePart);
                        }
                        else
                        {
                            for (int i = Part; i >= 0; i--)
                                ScanGroupList[i].transform.position = StartPos +                // 開始位置
                                    (Part - i) * new Vector3(MoveOffset, MoveOffset);           // 每個的 Offset

                            ScanBackgroundList[Part].color = new Color(1, 1, 1, 1);
                            ScanResultList[Part].color = new Color(1, 1, 1, 1);
                            float FillAmount = Mathf.Clamp01((Percentage - 0.2f * Part - MoveBackTimePart) / (0.2f - MoveBackTimePart));
                            ScanResultList[Part].fillAmount = FillAmount;
                        }
                    }
                    else
                        ScanResultList[0].fillAmount = 0.8f * (Percentage - 0.2f * Part) / 0.2f + 0.2f;
                    break;
                }
            case ScanAreaState.ANIM_STATE:
                {
                    float SingleShotTime = (EndTime - AnimTime) / (AreaSize - 5);
                    int Part = (int)((counterT - AnimTime) / SingleShotTime) + 5;

                    // 前半段
                    for (int i = Part; i >= 0; i--)
                        if (Part - FladeOutEnd < i && i <= Part - FladeOutStart)
                        {
                            ScanBackgroundList[i].color = new Color(1, 1, 1, (float)(i - (Part - FladeOutEnd)) / (FladeOutEnd - FladeOutStart));
                            ScanResultList[i].color = new Color(1, 1, 1, (float)(i - (Part - FladeOutEnd)) / (FladeOutEnd - FladeOutStart));
                        }
                        else if (Part - FladeOutEnd >= i)
                        {
                            ScanBackgroundList[i].color = new Color(1, 1, 1, 0);
                            ScanResultList[i].color = new Color(1, 1, 1, 0);
                        }
                        else
                        {
                            ScanBackgroundList[i].fillAmount = 1;
                            ScanResultList[i].fillAmount = 1;
                        }

                    ScanGroupList[Part].SetActive(true);

                    // 後移 & 淡進的效果
                    if (counterT - AnimTime - SingleShotTime * (Part - 5) < MoveBackTime)
                    {
                        float MoveBackTime_Percentage = (counterT - AnimTime - SingleShotTime * (Part - 5)) / MoveBackTime;

                        // 後移
                        for (int i = Part - 1; i >= 0; i--)
                            ScanGroupList[i].transform.position = StartPos +                // 開始位置
                                ((Part - 1) - i) * new Vector3(MoveOffset, MoveOffset) +    // 每個的 Offset
                                new Vector3(MoveOffset, MoveOffset) * MoveBackTime_Percentage;    // 正在移動的動畫

                        // 淡進
                        ScanGroupList[Part].SetActive(true);
                        ScanBackgroundList[Part].color = new Color(1, 1, 1, MoveBackTime_Percentage);
                        ScanResultList[Part].color = new Color(1, 1, 1, MoveBackTime_Percentage);
                    }
                    else
                    {
                        // 後移
                        for (int i = Part; i >= 0; i--)
                            ScanGroupList[i].transform.position = StartPos +                // 開始位置
                                (Part - i) * new Vector3(MoveOffset, MoveOffset);           // 每個的 Offset 

                        ScanBackgroundList[Part].color = new Color(1, 1, 1, 1);
                        ScanResultList[Part].color = new Color(1, 1, 1, 1);

                        float StartTime = SingleShotTime * (Part - 5) + MoveBackTime;
                        float EndTime = SingleShotTime * (Part - 5 + 1);
                        float FillAmount = ((counterT - AnimTime - SingleShotTime * (Part - 5) - MoveBackTime) / (EndTime - StartTime));
                        ScanResultList[Part].fillAmount = FillAmount;
                    }
                    break;
                }
            case ScanAreaState.END_STATE:
                {
                    // 後移
                    for (int i = AreaSize - 1; i >= 0; i--)
                        ScanGroupList[i].transform.position = StartPos +                // 開始位置
                            ((AreaSize - 1 - 1) - i) * new Vector3(MoveOffset, MoveOffset);    // 每個的 Offset

                    // 前半段
                    for (int i = AreaSize - 1; i >= 0; i--)
                        if (AreaSize - 1 - FladeOutEnd < i && i <= AreaSize - 1 - FladeOutStart)
                        {
                            ScanBackgroundList[i].color = new Color(1, 1, 1, (float)(i - (AreaSize - FladeOutEnd)) / (FladeOutEnd - FladeOutStart));
                            ScanResultList[i].color = new Color(1, 1, 1, (float)(i - (AreaSize - FladeOutEnd)) / (FladeOutEnd - FladeOutStart));
                        }
                        else if (AreaSize - 1 - FladeOutEnd >= i)
                        {
                            ScanBackgroundList[i].color = new Color(1, 1, 1, 0);
                            ScanResultList[i].color = new Color(1, 1, 1, 0);
                        }
                        else
                        {
                            ScanBackgroundList[i].fillAmount = 1;
                            ScanResultList[i].fillAmount = 1;
                        }
                    break;
                }
        }

    }
}
