using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ASCAnimState
{
    DISAPPER_STATE = 0,
    SHOW_STATE,
    ANIM_STATE_WITHOUT_GYRO1,
    ANIM_STATE_WITHOUT_GYRO_STOP1,
    ANIM_STATE_WITHOUT_GYRO2,
    ANIM_STATE_WITHOUT_GYRO_STOP2,
    ANIM_STATE_WITH_GYRO_TOPOSITION,
    ANIM_STATE_WITH_GYRO,
    ANIM_STATE_WITH_GYRO_STOP,
    ANIM_STATE_ALIGNMENT,
    END_STATE
}


public class ASCReader : MonoBehaviour
{
    [Header("===== 檔案 =====")]
    public TextAsset FileData;
    public Vector3 OrgOffset;
    public Color color = Color.red;
    public Vector3 rot = new Vector3(45, 180, 0);

    [Header("===== 動畫 =====")]
    public ASCAnimState state = ASCAnimState.DISAPPER_STATE;
    public float ShowTime;
    public float AnimTime_WithoutGyro1;
    public float AnimTime_WithoutGyro_Stop1;
    public float AnimTime_WithoutGyro2;
    public float AnimTime_WithoutGyro_Stop2;
    public float AnimTime_WithGyro_ToPosition;
    public float AnimTime_WithGyro;
    public float AnimTime_WithGyro_Stop;
    public float AnimTime_Alignment;
    public float EndTime;
    private float counterTime = 0;
    private bool IsDraw = false;

    [Header("===== 參數 =====")]
    public Vector3 WithoutGyro1;
    public Vector3 WithoutGyro2;
    public Vector3 WithoutGyro2_Angle;
    public Vector3 WithGyro_Angle;
    public Vector3 WithGyro;

    // 劃出來所需要的 DataType
    private List<Vector3> DataArray = new List<Vector3>();
    private Vector3 MidPoint = new Vector3(0, 0, 0);


    [Header("===== Debug =====")]
    public Vector3 TempOffset = new Vector3(0, 0, 0);
    public Quaternion TempRot;

    // 一些常數
    private int indexOffset = 25;
    private float PointSize = 0.01f;
    private float ScaleFactor = 0.6f;

    private void Start()
    {
        #region 讀檔案
        string data = FileData.text;
        string[] linesData = data.Split('\n');

        float a, b, c;
        for(int i = 0; i < linesData.Length; i++)
        {
            string[] lineData = linesData[i].Split(' ');
            if(i >= 2 && lineData.Length >= 3)
            {
                a = float.Parse(lineData[0]);
                b = float.Parse(lineData[1]);
                c = float.Parse(lineData[2]);

                Vector3 point = new Vector3(a, b, c);
                DataArray.Add(point);

                MidPoint += point;
            }
        }
        MidPoint /= DataArray.Count;
        TempOffset = OrgOffset;
        TempRot = Quaternion.Euler(rot);
        #endregion
    }

    public void Update()
    {
        counterTime += Time.deltaTime;

        float tempT = 0;
        switch (state)
        {
            #region 不顯示的狀態
            case ASCAnimState.DISAPPER_STATE:
                {
                    if (counterTime >= ShowTime)
                    {
                        state = ASCAnimState.SHOW_STATE;
                        IsDraw = true;
                    }
                    break;
                }
            #endregion
            #region 顯示 但不動
            case ASCAnimState.SHOW_STATE:
                {
                    if (counterTime >= AnimTime_WithoutGyro1)
                        state = ASCAnimState.ANIM_STATE_WITHOUT_GYRO1;
                    break;
                }
            #endregion
            #region 顯示 隨機 Gyro 拼接
            case ASCAnimState.ANIM_STATE_WITHOUT_GYRO1:
                {
                    tempT = (counterTime - AnimTime_WithoutGyro1) / (AnimTime_WithoutGyro_Stop1 - AnimTime_WithoutGyro1);
                    TempOffset = Vector3.Slerp(OrgOffset, WithoutGyro1, tempT);

                    if (counterTime >= AnimTime_WithoutGyro_Stop1)
                    {
                        TempOffset = WithoutGyro1;
                        state = ASCAnimState.ANIM_STATE_WITHOUT_GYRO_STOP1;
                    }
                    break;
                }
            case ASCAnimState.ANIM_STATE_WITHOUT_GYRO_STOP1:
                {
                    if (counterTime >= AnimTime_WithoutGyro2)
                        state = ASCAnimState.ANIM_STATE_WITHOUT_GYRO2;
                    break;
                }
            #endregion
            #region 顯示 隨機 Gyro 拼接2
            case ASCAnimState.ANIM_STATE_WITHOUT_GYRO2:
                {
                    tempT = (counterTime - AnimTime_WithoutGyro2) / (AnimTime_WithoutGyro_Stop2 - AnimTime_WithoutGyro2);
                    TempOffset = Vector3.Slerp(WithoutGyro1, WithoutGyro2, tempT);
                    TempRot = Quaternion.Slerp(Quaternion.Euler(rot), Quaternion.Euler(WithoutGyro2_Angle), tempT);

                    if (counterTime >= AnimTime_WithoutGyro_Stop2)
                    {
                        TempOffset = WithoutGyro2;
                        TempRot = Quaternion.Euler(WithoutGyro2_Angle);
                        state = ASCAnimState.ANIM_STATE_WITHOUT_GYRO_STOP2;
                    }
                    break;
                }
            case ASCAnimState.ANIM_STATE_WITHOUT_GYRO_STOP2:
                {
                    if (counterTime >= AnimTime_WithGyro_ToPosition)
                    {
                        TempOffset = OrgOffset;
                        TempRot = Quaternion.Euler(rot);
                        state = ASCAnimState.ANIM_STATE_WITH_GYRO_TOPOSITION;
                    }
                    break;
                }
            #endregion
            #region 回到一開始
            case ASCAnimState.ANIM_STATE_WITH_GYRO_TOPOSITION:
                {
                    if(counterTime >= AnimTime_WithGyro)
                        state = ASCAnimState.ANIM_STATE_WITH_GYRO;
                    break;
                }
            #endregion
            #region 先轉 在拼接
            case ASCAnimState.ANIM_STATE_WITH_GYRO:
                {
                    tempT = (counterTime - AnimTime_WithGyro) / (AnimTime_WithGyro_Stop - AnimTime_WithGyro);
                    TempRot = Quaternion.Slerp(Quaternion.Euler(rot), Quaternion.Euler(WithGyro_Angle), tempT);
                    if (counterTime >= AnimTime_WithGyro_Stop)
                    {

                        TempRot = Quaternion.Euler(WithGyro_Angle);
                        state = ASCAnimState.ANIM_STATE_ALIGNMENT;
                    }
                    break;
                }
            case ASCAnimState.ANIM_STATE_ALIGNMENT:
                {
                    tempT = (counterTime - AnimTime_WithGyro_Stop) / (AnimTime_Alignment - AnimTime_WithGyro_Stop);
                    TempOffset = Vector3.Slerp(OrgOffset, WithGyro, tempT);
                    if (counterTime >= AnimTime_Alignment)
                    {

                        TempOffset = WithGyro;
                        state = ASCAnimState.END_STATE;
                    }
                    break;
                }
                #endregion
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        if(IsDraw)
            for(int i = 0; i < DataArray.Count; i+= indexOffset)
            {
                Vector3 tempPoint = TempRot * (DataArray[i] - MidPoint) * ScaleFactor + MidPoint;
                Gizmos.DrawSphere(TempOffset + tempPoint, PointSize);
            }
    }
}
