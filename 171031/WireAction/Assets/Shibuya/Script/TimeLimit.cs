using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeLimit : MonoBehaviour {


    private Text m_Text;
    //制限時間
    public float timeLimit_sec;
    //コロン
    private string coron;
    //カウントストップ
    private bool isCountStop;
    void Start () {
        m_Text = GetComponent<Text>();
        isCountStop = false;
        coron = " : ";
    }
	
	// Update is called once per frame
	void Update () {
        if (isCountStop == false)
        {
            timeLimit_sec -= Time.deltaTime;
        }
        //制限時間が0以下にならないように
        if (timeLimit_sec < 0) timeLimit_sec = 0;
        //制限時間表示
        m_Text.text = ConvertTime(timeLimit_sec);
    }
    //秒を分と秒に分けて表示するために変換
    private string ConvertTime(float timeLimit)
    {
        //分
        int min = (int)timeLimit_sec / 60;
        //秒
        float sec = timeLimit_sec - (min * 60);

        return "[Time]" + min + coron + string.Format("{0:00}",(int)sec);
    }
    //制限時間が来たか？
    public bool Is_TimeLimit()
    {
        if(timeLimit_sec <= 0)
        {
            return true;
        }
        return false;
    }
    //カウントストップ
    public void CountStop()
    {
        isCountStop = true;
    }
}
