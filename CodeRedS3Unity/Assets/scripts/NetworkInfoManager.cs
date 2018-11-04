using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class NetworkedDevice{
    public int bytesIn;
    public int bytesOut;
    public string id;
}

[System.Serializable]
public class NetworkInfo
{
    public string id;
    private int upSpeed = 0;
    private int downSpeed = 0;

    public NetworkInfo(string id){
        this.id = id;
        upSpeed = 0;
        downSpeed = 0;
    }

    public void SetUpSpeed(int speed){
        upSpeed = speed;
    }

    public void SetDownSpeed(int speed)
    {
        downSpeed = speed;
    }

    public string GetUpDown()
    {
        return "Up=" + upSpeed + "B/s, Down= " + downSpeed + "B/s";
    }
}

public class NetworkInfoManager : MonoBehaviour {

    public static NetworkInfoManager Instance;
    private string serverUrl = "https://us-central1-codered-451b3.cloudfunctions.net/";

    [SerializeField]
    private List<NetworkInfo> devices;
    private float timer = 0f;

    [SerializeField]
    private Text t_upDown;

    private Graph currentGraph = null;

    private void Start()
    {
        Instance = this;
        devices = new List<NetworkInfo>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 1f)
        {
            if (SimpleCloudHandler.Instance.canGetData)
            {
                StartCoroutine(GetText());
                timer = 0;
            }
        }
    }

    IEnumerator GetText()
    {
        UnityWebRequest networkInfoWww = UnityWebRequest.Get(serverUrl + "getClientUsage?clientId=" + SimpleCloudHandler.Instance.currentId);
		
        yield return networkInfoWww.SendWebRequest();
		
        if (networkInfoWww.isNetworkError || networkInfoWww.isHttpError)
            Debug.Log(networkInfoWww.error);
        else
        {
            Debug.Log(networkInfoWww.downloadHandler.text);
            NetworkedDevice temp = JsonUtility.FromJson<NetworkedDevice>(networkInfoWww.downloadHandler.text);
            if (DeviceIdExists(temp.id))
                UpdateNetworkUsage(temp.id, temp.bytesOut, temp.bytesIn);
            else
                AddNetworkDevice(temp.id);
        }
    }

    public bool DeviceIdExists(string id){
        if (devices.Count > 0)
        {
            foreach (NetworkInfo netInfo in devices)
                if (netInfo.id == id)
                    return true;
        }

        return false;
    }

    public void AddNetworkDevice(string id){
        if(DeviceIdExists(id)){
            Debug.Log("That id is already used");
            return;
        }

        NetworkInfo temp = new NetworkInfo(id);
        devices.Add(temp);
    }

    public void UpdateNetworkUsage(string id, int upSpeed, int downSpeed){
        var netInfo = GetNetworkInfo(id);

        netInfo.SetUpSpeed(upSpeed);
        netInfo.SetDownSpeed(downSpeed);

        t_upDown.text = netInfo.GetUpDown();

        if(currentGraph != null)
            currentGraph.UpdateGraph(downSpeed, upSpeed);
    }

    public NetworkInfo GetNetworkInfo(string id){
        foreach (NetworkInfo netInfo in devices)
            if (netInfo.id == id)
                return netInfo;

        return null;
    }

    public void SetCurrentGraph(Graph graph){
        currentGraph = graph;
    }

    public bool GraphExists()
    {
        return currentGraph != null;
    }

    public GameObject GetCurrentGraphGO(){
        return currentGraph.gameObject;
    }
}
