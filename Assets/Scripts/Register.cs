using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.Networking;

public struct UserData
{
    public string username, password, nickname;
}

//서버로 부터 응답 받는 구조체와 공용체
public struct LoginResult
{
    public int result;
}
public enum ResponseType
{
    INVALID_USERNAME= 0,
    INVALID_PASSWORD,
    SUCCESS
}

public class Register : MonoBehaviour {

    #region LoginFiled
    public InputField Lid;
    public InputField Lpw;
    public GameObject Loginpanel;
    public GameObject Regpanel;
    public Button loginBtn;
    #endregion

    #region RegisterFiled
    public InputField Sid;
    public InputField Spw;
    public InputField Spwc;
    public InputField Sname;
    #endregion

    private void Start()
    {
        Regpanel.SetActive(false);
        loginBtn.interactable = false;  //interactable 활성화/비활성화
    }

    //로그인 버튼 클릭
    public void ClickLoginBtn()
    {
        loginBtn.interactable = false;

        string id = Lid.text;
        string pw = Lpw.text;
        if(string.IsNullOrEmpty(id)|| string.IsNullOrEmpty(pw))
        {
            return;
        }
        UserData ud = new UserData();
        ud.username = id;
        ud.password = pw;
        StartCoroutine(SignIn(ud));
        
        
    }

    //로그인 창에서 회원가입 버튼 클릭
    public void ClickOnRegPanel()
    {
        Regpanel.SetActive(true);
        Loginpanel.SetActive(false);
    }

    //회원가입창에서 회원가입완료 버튼 클릭시.
    public void ClickRegOk()
    {
        string id = Sid.text;
        string pw = Spw.text;
        string pwc = Spwc.text;
        string name = Sname.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(pwc) || string.IsNullOrEmpty(name)) 
        {
            return;
        }
        if(pw.Equals(pwc))
        {
            //서버에 전송
            UserData ud = new UserData();
            ud.username = id;
            ud.password = pw;
            ud.nickname = name;
            StartCoroutine(SignUp(ud));
            Regpanel.SetActive(false);
            Loginpanel.SetActive(true);

            Debug.Log("회원가입완료.. 회원의 ID는 " + ud.username + "이며 pw는 "+ ud.password + " 회원의 이름은 "+ ud.nickname + " 입니다.");
        }
    }
    //x 버튼
    public void CloseBtn()
    {
        Regpanel.SetActive(false);
        Loginpanel.SetActive(true);
    }

    public void UpdateLoginInputFiled()
    {
        if (!string.IsNullOrEmpty(Lid.text) && !string.IsNullOrEmpty(Lpw.text))
        {
            loginBtn.interactable = true;
        }
        else
        {
            loginBtn.interactable = false;
        }
    }

    IEnumerator SignUp(UserData userdata)
    {
        string postData = JsonUtility.ToJson(userdata);
        byte[] sendData = Encoding.UTF8.GetBytes(postData);

        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost:3000/users/add", postData))
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
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    IEnumerator SignIn(UserData userdata)
    {
        string SaveUserName = userdata.username;

        string postData = JsonUtility.ToJson(userdata);
        byte[] sendData = Encoding.UTF8.GetBytes(postData);

        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost:3000/users/find", postData))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.Send();
            loginBtn.interactable = true;

            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);

            }
            else
            {
                string resultStr = www.downloadHandler.text;
                
                var result = JsonUtility.FromJson<LoginResult>(resultStr);

                if (result.result == 2)
                {
                    PlayerPrefs.SetString("UserName",SaveUserName);
                    SceneManager.LoadScene("InGame");
                }
                else if(result.result == 0)
                {
                    Debug.LogError("아이디가 존재하지 않습니다. 확인해주세요");
                }
                else if (result.result == 1)
                {
                    Debug.LogError("패스워드가 다릅니다. 확인해주세요");
                }

            }
        }
    }


}
