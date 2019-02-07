using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASCReader : MonoBehaviour
{
    [Header("===== 檔案 =====")]
    public TextAsset FileData;
    public Vector3 Offset;
    public Color color = Color.red;
    public Vector3 rot = new Vector3(45, 180, 0);

    // 劃出來所需要的 DataType
    private List<Vector3> DataArray = new List<Vector3>();
    private Vector3 MidPoint = new Vector3(0, 0, 0);

    // 一些常數
    private int indexOffset = 50;
    private float PointSize = 0.01f;
    private float ScaleFactor = 0.2f;

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

        // 調整 Size
        for(int i = 0; i < DataArray.Count; i++)
            DataArray[i] = Quaternion.Euler(rot) * (DataArray[i] - MidPoint) * ScaleFactor + MidPoint;
        #endregion
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        for(int i = 0; i < DataArray.Count; i+= indexOffset)
        {
            Gizmos.DrawSphere(Offset + DataArray[i], PointSize);
        }
    }
}
