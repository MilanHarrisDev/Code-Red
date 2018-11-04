using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour {

    [SerializeField]
    private Text t_deviceTitle;

    [SerializeField]
    private int cacheSize = 15;                        //amount of time in seconds that is used for averages
    [SerializeField]
    private float maxHeight = 5;
    private int heightCeiling;
    [SerializeField]
    private GameObject downloadGraphic, uploadGraphic; //graphics are cubes
    [SerializeField]
    private Text t_download, t_upload,
    t_downloadAverage, t_downloadLow, t_downloadHigh,  //averages are based on
    t_uploadAverage, t_uploadLow, t_uploadHigh;        //last x seconds (uses cacheSize variable)
    [SerializeField]
    private Text t_heightCeiling;

    private int updateCount = 0;

    private List<Vector2> speeds = new List<Vector2>();

    public void Init(string deviceName){
        t_deviceTitle.text = deviceName;
    }

    public void UpdateGraph(int downSpeed, int upSpeed){
        t_download.text = downSpeed.ToString();
        t_upload.text = upSpeed.ToString();

        speeds.Insert(0, new Vector2(downSpeed, upSpeed));

        if (speeds.Count > 15)
            speeds.RemoveAt(15);

        int downloadLow = 1000000000;
        int downloadHigh = 0;
        int uploadLow = 1000000000;
        int uploadHigh = 0;
        int downloadTotal = 0;
        int uploadTotal = 0;

        foreach(Vector2 speed in speeds)
        {
            int download = (int)speed.x;
            int upload = (int)speed.y;

            downloadLow = download < downloadLow ? download : downloadLow;
            downloadHigh = download > downloadHigh ? download : downloadHigh;
            uploadLow = upload < uploadLow ? upload : uploadLow;
            uploadHigh = upload > uploadHigh ? upload : uploadHigh;
            downloadTotal += download;
            uploadTotal += upload;
        }

        //t_downloadAverage.text = ((float)downloadTotal / speeds.Count).ToString();
        //t_uploadAverage.text = ((float)uploadTotal / speeds.Count).ToString();
        //t_uploadLow.text = uploadLow.ToString();
        //t_downloadLow.text = downloadLow.ToString();
        //t_downloadHigh.text = downloadHigh.ToString();
        //t_uploadHigh.text = uploadHigh.ToString();

        if (speeds.Count > 1)
        {
            heightCeiling = downloadHigh;
            if (heightCeiling < uploadHigh)
                heightCeiling = uploadHigh;
        }
        else
            heightCeiling = downSpeed;

        t_heightCeiling.text = ((float)heightCeiling / 1024).ToString("0.0") + " kB/s";

        downloadGraphic.transform.localScale = new Vector3(.5f, .5f, ((float)downSpeed / (float)heightCeiling) * maxHeight * .5f);
        uploadGraphic.transform.localScale = new Vector3(.5f, .5f, ((float)upSpeed / (float)heightCeiling) * maxHeight * .5f);

        updateCount++;
    }
}
