using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;

public struct DownloadData
{
    public string username;
    public string nickname;
    public double score;
}

public class UserInfo : MonoBehaviour {
    private string userName;

    public Text idText;
    public Text nickText;
    public Text scoreText;
    
    public GameObject statPanel;

    public void Start()
    {
        statPanel.SetActive(false);
        userName = PlayerPrefs.GetString("UserName");
    }

    public void StatusClick()
    {
        UserData ud = new UserData();
        ud.username = userName;
        if (!statPanel.activeSelf)
        {
            statPanel.SetActive(true);
        }
        else
        {
            statPanel.SetActive(false);
        }
        StartCoroutine(Serch(ud));
    }

    IEnumerator Serch(UserData userdata)
    {
        //Debug.Log(username);
        string postData = JsonUtility.ToJson(userdata);
        byte[] sendData = Encoding.UTF8.GetBytes(postData);

        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost:3000/users/serch", postData))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.Send();

            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string dataStr = www.downloadHandler.text;
                var data = JsonUtility.FromJson<DownloadData>(dataStr);
               
                idText.text = data.username;
                nickText.text = data.nickname;
                scoreText.text = "" + data.score;
            }
        }
    }
}
