using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PCAnimState
{
    STATE_DISAPPER,
    STATE_ANIM1,
    STATE_ANIM,
    STATE_APPPER
}

public class PointCloudAnim : MonoBehaviour
{
    public TextAsset PCFile;
    public Vector3 EulerAngle = new Vector3(0, 0, 0);
    public Color PCColor;
    

    // 常數
    private List<Vector3> PCList = new List<Vector3>();
    private Vector3 LastOffsetPoint = new Vector3(0, 0, 0);
    private Vector3 MidPoint = new Vector3(0,0,0);
    private float ScaleSize = 1;
    private float PointSize = 0.06f;
    private int SkipOffset = 25;
    private float MinX = 100;
    private float MaxX = -100;
    private float ShowMount = 1;
    private PCAnimState state = PCAnimState.STATE_DISAPPER;

    [Header("===== 計數器 =====")]
    public float StartTime = 0;
    public float Anim1Time = 0;
    public float EndTime = 0;
    public float counter = 0;

    private void Start()
    {
        string data = PCFile.text;
        string[] linesData = data.Split('\n');

        float a, b, c;
        Vector3 MidPoint = new Vector3(0, 0, 0);
        for (int i = 1; i < linesData.Length; i++)
        {
            string[] lineData = linesData[i].Split(' ');
            if (i >= 2 && lineData.Length >= 3)
            {
                a = float.Parse(lineData[0]);
                b = float.Parse(lineData[1]);
                c = float.Parse(lineData[2]);

                Vector3 point = new Vector3(a, b, c) * ScaleSize;
                PCList.Add(point);

                MidPoint += point;
            }
        }
        MidPoint /= PCList.Count;
    }

    private void Update()
    {
        counter += Time.deltaTime;
        if (counter >= StartTime)
            state = PCAnimState.STATE_ANIM1;
        if (counter >= Anim1Time)
            state = PCAnimState.STATE_ANIM;
        if (counter >= EndTime)
            state = PCAnimState.STATE_APPPER;

        // 狀態更便
        switch (state)
        {
            case PCAnimState.STATE_DISAPPER:
                {
                    ShowMount = 0;
                }
                break;
            case PCAnimState.STATE_ANIM1:
                {
                    ShowMount = 0.4f;
                }
                break;
            case PCAnimState.STATE_ANIM:
                {
                    ShowMount = Mathf.Lerp(0.4f, 1, (counter - Anim1Time) / (EndTime - Anim1Time));
                }
                break;
            case PCAnimState.STATE_APPPER:
                {
                    ShowMount = 1;
                }
                break;
        }
    }

    private void OnDrawGizmos()
    {
        // 先算最大最小值
        MinX = 10000;
        MaxX = -10000;
        for (int i = 0; i < PCList.Count; i += SkipOffset)
        {
            Vector3 pos = this.transform.position;
            Quaternion quat = this.transform.rotation * Quaternion.Euler(EulerAngle);
            pos += quat * (PCList[i] - MidPoint);

            MinX = Mathf.Min(pos.x, MinX);
            MaxX = Mathf.Max(pos.x, MaxX);
        }

        Gizmos.color = PCColor;

        for (int i = 0; i < PCList.Count; i+= SkipOffset)
        {
            Vector3 pos = this.transform.position;
            Quaternion quat = this.transform.rotation * Quaternion.Euler(EulerAngle);
            pos += quat * (PCList[i] - MidPoint);
            
            float valueX = (MaxX - MinX) * ShowMount + MinX;

            if (pos.x < valueX)
                Gizmos.DrawSphere(pos, PointSize);
        }
    }
}
