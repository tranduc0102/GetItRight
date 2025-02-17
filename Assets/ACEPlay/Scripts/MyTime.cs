///Ver2.9 22/12/2023
///Thêm CultureInfo.InvariantCulture cho các hàm format DateTime để không bị lỗi trên device sử dụng Hindi
///Ver2.8 17/07/2023
///Thêm AsTimeAgo
///Ver2.7 05/05/2023
///Thêm isByUTC để cho phép đếm ngược theo thời gian UTC
///Ver2.6 12/01/2023
///Thêm Âm lịch
///Ver2.5 28/12/2022
///Thêm tính năng auto fit format cho hàm display time
///Ver2.4 28/10/2022
///Fix lỗi time countDown không chạy trong nền.
///Ver2.3 25/10/2022
///check with urls
///Ver2.2 16/08/2022
///fix lỗi format khiến việc lưu time rồi load lên bị sai => lỗi phần heart (phần lưu và phần load bị lộn giữa ngày và tháng)
///Ver2.1 20/06/2022
///fix formatTimeSpan_hhh_mm_ss => display 00=>60 ví dụ: Debug.Log(hhh_mm_ss(1319.953));
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Globalization;

[System.Serializable]
public class MyIntEvent : UnityEvent<int> { }

public class MyTime : MonoBehaviour
{
    public static MyTime instance;
    public bool isAllowGetTimeOnline = false;
    public bool isDoneStart = false;
    #region time online
    public bool isFirstLoginNewDay = false;
    public bool isFirstLoginNewWeek = false;
    public bool isCheckedTimeOnline = false;
    DateTime timeOnline;
    [SerializeField] float deltaTime;
    //string format = "dd-MM-yyyy/HH:mm:ss";
    /// <summary>
    /// Output: 18/03/2020 12:38:56
    /// </summary>
    public readonly string format = "dd/MM/yyyy HH:mm:ss";//18/03/2020 12:38:56
    /// <summary>
    /// Output: 18 03 2020
    /// </summary>
    public readonly string formatShortDateDayMonthYear = "dd MM yyyy";//18 03 2020
    /// <summary>
    /// Output: June
    /// </summary>
    public readonly string formatMonthFull = "MMMM";//June
    /// <summary>
    /// Output: Jun 15
    /// </summary>
    public readonly string formatShortDate = "MMM d";//Jun 15
    /// <summary>
    /// Output: 03 2022
    /// </summary>
    public readonly string formatShortDayYear = "MM yyyy";//Jun 15
    public readonly string formatTimeSpan = @"d\d\,hh\:mm\:ss";//dd\.hh\:mm\:ss  //new TimeSpan(2, 25, 30, 30) => 3d,01:30:30
    /// <summary>
    /// new TimeSpan(2, 25, 30, 30) => 3d 01h
    /// </summary>
    public readonly string formatTimeSpanDayHour = @"%d\d\ hh\h";//3d 01h
    /// <summary>
    /// new TimeSpan(0, 0, 30, 30) => 30:30
    /// </summary>
    public readonly string formatTimeSpanMS = @"mm\:ss";
    /// <summary>
    /// new TimeSpan(2, 25, 30, 30) => 01:30:30
    /// </summary>
    public readonly string formatTimeSpanHMS = @"hh\:mm\:ss";
    public readonly string formatTimeSpanHM = @"hh\h mm\m";
    //public readonly string formatTimeSpan_hhh_mm_ss = "hhh:mm:ss";
    private void Awake()
    {
        //isAllowGetTimeOnline = false;
        if (instance == null)
        {
            instance = this;

            deltaTime = Time.realtimeSinceStartup;
            timeOnline = DateTime.Now;
        }
        else Destroy(this);
    }

    string hhh_mm_ss(double seconds)
    {
        seconds = Math.Round(seconds, 0);
        double sec = seconds % 60;
        double min = ((seconds - sec) / 60) % 60;
        double hhh = (seconds - sec - 60 * min) / 3600;
        return hhh > 0 ? string.Format(CultureInfo.InvariantCulture, "{2}:{1:00}:{0:00}", sec, min, hhh)
                       : min.ToString("00", CultureInfo.InvariantCulture) + ":" + sec.ToString("00", CultureInfo.InvariantCulture);
    }
    [SerializeField] private string[] urls = new string[] { "https://www.google.com", "https://www.wikipedia.com", "https://www.amazon.com", "https://y.qq.com", "https://www.microsoft.com" };

    private bool isPauseApp;
    private float timePauseApp;
    private bool isFirst;

    public string GetHostNameOrAddress(int index)
    {
        index = index % urls.Length;
        return urls[index].Replace("https://", "");
    }

    private void OnApplicationPause(bool pause)
    {
        if (!isFirst)
        {
            isFirst = true;
            return;
        }
        if (pause)
        {
            timePauseApp = Time.realtimeSinceStartup;
            temp = GetCurrentTime();
        }
        else
        {
            if (CoroutineWaitResetIsPauseApp != null) StopCoroutine(CoroutineWaitResetIsPauseApp);
            isPauseApp = true;
            CoroutineWaitResetIsPauseApp = StartCoroutine(WaitResetIsPauseApp());
        }
    }


    DateTime temp;
    Coroutine CoroutineWaitResetIsPauseApp;
    IEnumerator WaitResetIsPauseApp()
    {
        if (isAllowGetTimeOnline)
        {
            float time = Time.realtimeSinceStartup;
            StartCoroutine(GetTimeOnline(0.5f, result =>
            {
                DOVirtual.DelayedCall(Time.realtimeSinceStartup - time >= 1.5f ? 0 : 1.5f - (Time.realtimeSinceStartup - time), delegate
                {
                    deltaTime = Time.realtimeSinceStartup;
                    timeOnline = result;
                    timePauseApp = Time.realtimeSinceStartup - timePauseApp;
                    isPauseApp = false;
                });
            }));
        }
        else
        {
            DateTime mytime = GetCurrentTime();
            if (Mathf.Abs((int)(mytime - DateTime.Now).TotalSeconds) > 60)
            {
                Debug.LogError("Phát hiện hack time bằng cách chỉnh setting!");
            }
            yield return new WaitForSecondsRealtime(1.5f);    
            timePauseApp = Time.realtimeSinceStartup - timePauseApp;
            //timePauseApp = (float)(DateTime.Now - temp).TotalSeconds;
            isPauseApp = false;
        }
    }

    private IEnumerator GetTimeOnline(float waitAgain, Action<DateTime> onDone)
    {
        bool isAgain = false;
        int index = 0;
        DateTime resultTime = default;
    Again:
        if (isAgain) yield return new WaitForSeconds(waitAgain);
        UnityWebRequest myHttpWebRequest = UnityWebRequest.Get(urls[index]);
        yield return myHttpWebRequest.SendWebRequest();
        try
        {
            string netTime = myHttpWebRequest.GetResponseHeader("date");
            string timezone = DateTimeOffset.Now.ToString("M/d/yyyy h:mm:ss tt zzz", CultureInfo.InvariantCulture);//3/11/2020 9:55:23 AM +07:00
            timezone = timezone.Remove(0, timezone.Length - 6);//+07:00
            resultTime = DateTime.Parse(netTime).AddMinutes(BuTruChechLechMuiGio());
            if (onDone != null) onDone.Invoke(resultTime);
        }
        catch
        {
            index++;
            if (index >= urls.Length)
                index = 0;
            isAgain = true;
            goto Again;
        }
    }

    public string AsTimeAgo(DateTime dateTime, bool isUTC)
    {
        TimeSpan timeSpan = isUTC ? GetCurrentTime().ToUniversalTime().Subtract(dateTime) : GetCurrentTime().Subtract(dateTime);

        if (timeSpan.TotalSeconds < 60)
        {
            return $"{timeSpan.Seconds} sec ago";
        }
        else
        {
            if (timeSpan.TotalMinutes < 60)
            {
                return $"{timeSpan.Minutes} min {timeSpan.Seconds} sec ago";
            }
            else
            {
                if (timeSpan.TotalHours < 24)
                {
                    return $"{timeSpan.Hours} h {timeSpan.Minutes} min ago";
                }
                else
                {
                    return $"{timeSpan.Days} d {timeSpan.Hours} h ago";
                }
            }
        };
    }

    private IEnumerator Start()
    {
        if (!PlayerPrefs.HasKey("timeZone"))
        {
            string tz = DateTimeOffset.Now.ToString("M/d/yyyy h:mm:ss tt zzz", CultureInfo.InvariantCulture);
            tz = tz.Remove(0, tz.Length - 6);
            PlayerPrefs.SetString("timeZone", tz);
        }

        if (isAllowGetTimeOnline)
        {
            bool isAgain = false;
            int index = 0;
            string timezone = DateTimeOffset.Now.ToString("M/d/yyyy h:mm:ss tt zzz", CultureInfo.InvariantCulture);//3/18/2020 9:55:23 AM +07:00
            timezone = timezone.Remove(0, timezone.Length - 6);//+07:00
        Again:
            if (isAgain) yield return new WaitForSeconds(3.28f);
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get(urls[index]);
            yield return myHttpWebRequest.SendWebRequest();
            //try
            //{
            //    string netTime = myHttpWebRequest.GetResponseHeader("date");
            //    string timezone = DateTimeOffset.Now.ToString();//3/11/2020 9:55:23 AM +07:00
            //    deltaTime = Time.realtimeSinceStartup;
            //    timezone = timezone.Remove(0, timezone.Length - 6);//+07:00

            //    if (DateTime.TryParseExact(netTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeOnline))
            //    {
            //        timeOnline = timeOnline.AddMinutes(BuTruChechLechMuiGio());
            //    }    
            //        //timeOnline = DateTime.TryParse(netTime).AddMinutes(BuTruChechLechMuiGio());
            //    TA.Extension.DebugLogEditor("Done get time online with index: " + index);
            //}
            //catch
            //{
            //    index++;
            //    if (index >= urls.Length)
            //        index = 0;
            //    isAgain = true;
            //    goto Again;
            //}
            string netTime = myHttpWebRequest.GetResponseHeader("date");

            if (DateTime.TryParse(netTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeOnline))
            {
                deltaTime = Time.realtimeSinceStartup;
                timeOnline = timeOnline.AddMinutes(BuTruChechLechMuiGio());
                isCheckedTimeOnline = true;
            }
            else
            {
                index++;
                if (index >= urls.Length)
                    index = 0;
                isAgain = true;
                goto Again;
            }
        }

        //Debug.Log(netTime);//Tue, 10 Mar 2020 08:59:19 GMT
        //Debug.Log(DateTime.Parse(netTime).AddMinutes(BuTruChechLechMuiGio()).ToString());//3/11/2020 11:19:00 AM
        //Debug.Log("GMT " + timezone);//Mui gio hien tai
        yield return new WaitWhile(() => !ACEPlay.Bridge.BridgeController.instance.isCheckFirebase);
        if (!PlayerPrefs.HasKey("FirstLoginDay"))
        {
            isFirstLoginNewDay = true;
            PlayerPrefs.SetString("FirstLoginDay", GetCurrentTimeStr());
        }
        else
        {
            if (CheckNewDay(GetTime(PlayerPrefs.GetString("FirstLoginDay"))))
            {
                isFirstLoginNewDay = true;
                PlayerPrefs.SetString("FirstLoginDay", GetCurrentTimeStr());
            }
        }
        if (!PlayerPrefs.HasKey("FirstLoginWeek"))
        {
            isFirstLoginNewWeek = true;
            PlayerPrefs.SetString("FirstLoginWeek", GetCurrentTimeStr(true));
        }
        else
        {
            if (CheckNewWeek(GetTime(PlayerPrefs.GetString("FirstLoginWeek"))))
            {
                isFirstLoginNewWeek = true;
                PlayerPrefs.SetString("FirstLoginWeek", GetCurrentTimeStr());
            }
        }


        if (isFirstLoginNewDay)
        {
            if (PlayerPrefs.GetInt("retention", -1) == -1)//D0
            {
                PlayerPrefs.SetInt("retention", 0);

                ACEPlay.Bridge.BridgeController.instance.SetPropertyRetendDay("0");
                PlayerPrefs.SetString("FirstLoginGameDay", GetCurrentTimeStr());//ngày đầu mở game
            }
            else
            {
                DateTime dateTime = GetTime(PlayerPrefs.GetString("FirstLoginGameDay"));//GetCurrentTime();
                DateTime dateTime2 = GetCurrentTime(); //dateTime.AddHours(0.1);

                TimeSpan timeSpan = dateTime2.Subtract(dateTime);

                int totalDay = (int)(timeSpan.TotalDays);

                if (totalDay < timeSpan.TotalDays)
                {
                    totalDay++;
                }
                PlayerPrefs.SetInt("retention", totalDay);

                ACEPlay.Bridge.BridgeController.instance.SetPropertyRetendDay(totalDay.ToString());
            }
        }
        isDoneStart = true;
        if (TA.ConnectInternetManager.instance == null)
        {
            var connectInternet = new GameObject("Connect Internet Manager", typeof(TA.ConnectInternetManager));
        }
    }

    int BuTruChechLechMuiGio()
    {
        if (PlayerPrefs.HasKey("timeZone"))
        {
            string timezone = DateTimeOffset.Now.ToString("M/d/yyyy h:mm:ss tt zzz", CultureInfo.InvariantCulture);//+07:00
            string timeZoneOld = PlayerPrefs.GetString("timeZone");//+07:00
            timezone = timezone.Remove(0, timezone.Length - 6);//+07:00

            if (!timezone.Equals(timeZoneOld))
            {
                int h = int.Parse(timezone.Remove(3, 3));
                int m = int.Parse(timezone.Remove(0, 4));
                int total = h * 60 + (h / h) * m;

                int h0 = int.Parse(timeZoneOld.Remove(3, 3));
                int m0 = int.Parse(timeZoneOld.Remove(0, 4));
                int total0 = h0 * 60 + (h0 / h0) * m0;

                //Debug.Log(total);
                //Debug.Log(total0);
                //Debug.Log(total0 - total);
                return total0 - total;
            }
            //Debug.Log("OK");
        }
        else
        {
            string tz = DateTimeOffset.Now.ToString("M/d/yyyy h:mm:ss tt zzz", CultureInfo.InvariantCulture);
            tz = tz.Remove(0, tz.Length - 6);
            PlayerPrefs.SetString("timeZone", tz);
            //Debug.Log("tzE " + tz);
        }
        return 0;
    }
    public bool CheckNewDay(DateTime timeOld)
    {
        DateTime now = GetCurrentTime();
        if ((now.Day > timeOld.Day && now.Month == timeOld.Month && now.Year == timeOld.Year)
            || (now.Month > timeOld.Month && now.Year == timeOld.Year)
            || now.Year > timeOld.Year)
        {//new Day
            Debug.Log("New Day!!");
            ACEPlay.Bridge.BridgeController.instance.TotalIAP_Day = 0;
            PlayerPrefs.SetInt("isShowedPackBonusInDay", 0);//Xoá cờ này để cho biết ngày hôm nay chưa show pack bonus
            //EvenADS.instance.Even_OpenApp();
            //int dayRetention = PlayerPrefs.GetInt("retention", -1);
            //dayRetention++;
            //PlayerPrefs.SetInt("retention", dayRetention);

            //ACEPlay.Bridge.BridgeController.instance.UserPropertyData("retent_type", dayRetention);



            return true;
        }
        return false;
    }
    public bool CheckNewWeek(DateTime timeOld)
    {
        do
        {
            timeOld = timeOld.AddDays(1);
        }
        while ((int)timeOld.DayOfWeek != 1);

        int compare = DateTime.Compare(timeOld, GetCurrentTime());

        if (compare <= 0)//DateTimeNextWeek < now
        {
            Debug.Log("New Week!!");
            return true;
        }
        return false;
    }
    public string GetStrTime(DateTime time, string format = null)
    {
        return time.ToString(string.IsNullOrEmpty(format) ? this.format : format, CultureInfo.InvariantCulture);
    }
    /// <summary>
    /// Lấy thời gian thực dưới dạng string.
    /// </summary>
    /// <param name="isResetTime"></param>
    /// <returns>isResetTime:
    /// <br>true: trả về 00:00:00 ngày hôm đó.</br>
    /// <br>false: trả về chính xác thời gian hiện tại.</br>
    /// </returns>
    public string GetCurrentTimeStr(bool isResetTime = false)
    {
        DateTime currentTime = /*isAllowGetTimeOnline ? */timeOnline.AddSeconds(Time.realtimeSinceStartup - deltaTime);// : DateTime.Now;
        if (isResetTime) { currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0); }//set timeOld về 0h ngày hôm đó
        return currentTime.ToString(format, CultureInfo.InvariantCulture);//dung de luu lai thoi gian cay trong duoc gieo
    }
    /// <summary>
    /// Lấy thời gian thực dưới dạng DateTime.
    /// </summary>
    /// <param name="isResetTime"></param>
    /// <returns>isResetTime:
    /// <br>true: trả về 00:00:00 ngày hôm đó.</br>
    /// <br>false: trả về chính xác thời gian hiện tại.</br>
    /// </returns>
    public DateTime GetCurrentTime(bool isResetTime = false)
    {
        DateTime currentTime = /*isAllowGetTimeOnline ? */timeOnline.AddSeconds(Time.realtimeSinceStartup - deltaTime);// : DateTime.Now;
        if (isResetTime) //set timeOld về 0h ngày hôm đó
        {
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
        }
        else
        {
            string strTime = GetStrTime(currentTime, format);
            //Debug.Log("strTimeCurrent " + strTime);
            currentTime = GetTime(strTime, format);
            //Debug.Log("currentTime " + currentTime.ToString(format));
        }
        return currentTime;//dung de luu lai thoi gian cay trong duoc gieo
    }

    public DateTime GetTime(string time, string format = null)
    {
        if (DateTime.TryParseExact(time, string.IsNullOrEmpty(format) ? this.format : format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultTime))
            return resultTime;//DateTime.ParseExact(time, string.IsNullOrEmpty(format) ? this.format : format, null);
        Debug.LogWarning($"Không thể Parse string time({time}) nên sẽ return currentTime!");
        return GetCurrentTime();
    }
    public TimeSpan TimeClaimRewardsAfkToNow()
    {
        try
        {
            DateTime DateTimeNow = GetCurrentTime();
            DateTime TimeClaimRewardsAFK;
            TimeSpan interval;
            TimeClaimRewardsAFK = DateTime.ParseExact(PlayerPrefs.GetString("TimeClaimRewardsAFK"), format, null);
            interval = DateTimeNow.Subtract(TimeClaimRewardsAFK);
            return interval;
        }
        catch (Exception e)
        {
            Debug.Log("====Error: " + e.Message);
            return TimeSpan.MinValue;
        }
    }

    public string DisplayTime(TimeSpan timeSpan, string format = null, bool isAutoNextFormat = false, bool isMinFormatMS = false)
    {
        if (timeSpan.TotalSeconds < 1) timeSpan = new TimeSpan(0, 0, 0);
        //if (format.Contains("hhh:mm:ss")) return hhh_mm_ss(timeSpan.TotalSeconds); 
        //if (format != null) return timeSpan.ToString(format);
        //return timeSpan.ToString(formatTimeSpanMS);
        //Debug.Log(string.Format("timeSpan: {0} format: {1}   {2}", timeSpan.ToString(), format, formatTimeSpanHMS));
        if (isAutoNextFormat)
        {
            if (timeSpan.Days > 0) return timeSpan.ToString(formatTimeSpanDayHour, CultureInfo.InvariantCulture);
            else
            {
                if (format == formatTimeSpanHMS && isMinFormatMS)
                {
                    return timeSpan.ToString(timeSpan.Hours > 0 ? formatTimeSpanHMS : formatTimeSpanMS, CultureInfo.InvariantCulture);
                }
                else if (format == formatTimeSpanMS) return timeSpan.ToString(formatTimeSpanMS, CultureInfo.InvariantCulture);
                return timeSpan.ToString(formatTimeSpanHMS, CultureInfo.InvariantCulture);
            }
        }
        else
        {
            if (format != null)
            {
                return timeSpan.ToString(format, CultureInfo.InvariantCulture);
            }
            return timeSpan.ToString(formatTimeSpanHMS, CultureInfo.InvariantCulture);
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.V)) Debug.Log(GetCurrentTimeStr());
    //    if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        DateTime DateTimeNow = GetCurrentTime();
    //        DateTime TimeClaimRewardsAFK;
    //        TimeSpan interval;
    //        TimeClaimRewardsAFK = DateTime.ParseExact("18/02/2021 14:30:11", format, null);
    //        interval = DateTimeNow.Subtract(TimeClaimRewardsAFK);
    //        Debug.Log(interval.ToString(formatTimeSpan));
    //        Debug.Log(interval.Add(TimeSpan.FromSeconds(1)).ToString(formatTimeSpan));
    //    }

    //}
    #endregion

    //public void CountDownTime(DateTime DateTimeNext, UILabel labelTime, string firstStr, string strColor, string format, UILabel labelTimeClone = null, Action onDone = null, MyIntEvent onIntEvent = null, UnityEvent onStop = null)
    //{
    //    var coroutineTime = StartCoroutine(TimeCountDown(DateTimeNext, labelTime, firstStr, strColor, format, labelTimeClone, onDone, onIntEvent));
    //    if (onStop != null) onStop.AddListener(delegate { StopCoroutine(coroutineTime); });
    //}
    public void CountDownTime(DateTime DateTimeNext, List<TextMeshProUGUI> labelTime, string firstStr, string strColor, string format, Action onDone = null, MyIntEvent onIntEvent = null, Action<Coroutine> onStop = null, bool isAutoNextFormat = false, bool isMinFormatMS = false, bool isByUTC = false)
    {
        if (onStop != null) onStop.Invoke(null);
        var coroutineTime = StartCoroutine(TimeCountDown(DateTimeNext, labelTime, firstStr, strColor, format, onDone, onIntEvent, isAutoNextFormat, isMinFormatMS, isByUTC));
        if (onStop != null)/* onStop.AddListener(delegate { StopCoroutine(coroutineTime); });*/
            onStop.Invoke(coroutineTime);
    }

    public void CountDownTime(DateTime DateTimeNext, Text labelTime, string firstStr, string strColor, string format, Text labelTimeClone = null, Action onDone = null, MyIntEvent onIntEvent = null, Action<Coroutine> onStop = null, bool isAutoNextFormat = false, bool isMinFormatMS = false, bool isByUTC = false)
    {
        if (onStop != null) onStop.Invoke(null);
        var coroutineTime = StartCoroutine(TimeCountDown(DateTimeNext, labelTime, firstStr, strColor, format, labelTimeClone, onDone, onIntEvent, isAutoNextFormat, isMinFormatMS, isByUTC));
        if (onStop != null)/* onStop.AddListener(delegate { StopCoroutine(coroutineTime); });*/
            onStop.Invoke(coroutineTime);
    }

    public void CountDownTime(DateTime DateTimeNext, TextMeshProUGUI labelTime, string firstStr, string strColor, string format, TextMeshProUGUI labelTimeClone = null, Action onDone = null, MyIntEvent onIntEvent = null, Action<Coroutine> onStop = null, bool isAutoNextFormat = false, bool isMinFormatMS = false, bool isByUTC = false)
    {
        if (onStop != null) onStop.Invoke(null);
        var coroutineTime = StartCoroutine(TimeCountDown(DateTimeNext, labelTime, firstStr, strColor, format, labelTimeClone, onDone, onIntEvent, isAutoNextFormat, isMinFormatMS, isByUTC));
        if (onStop != null) onStop.Invoke(coroutineTime); //onStop.AddListener(delegate { StopCoroutine(coroutineTime); });
    }

    IEnumerator TimeCountDown(DateTime DateTimeNext, List<TextMeshProUGUI> labelTime, string firstStr, string strColor, string format, Action onDone = null, MyIntEvent onIntEvent = null, bool isAutoNextFormat = false, bool isMinFormatMS = false, bool isByUTC = false)
    {
        labelTime.ForEach(tm => { tm.gameObject.SetActive(true); });
        DateTime DateTimeNow = isByUTC ? GetCurrentTime().ToUniversalTime() : GetCurrentTime();
        TimeSpan timeSpan;
        timeSpan = DateTimeNext.Subtract(DateTimeNow);
    Again:
        if (onIntEvent != null) onIntEvent.Invoke((int)timeSpan.TotalSeconds);

        labelTime.ForEach(tm => { tm.text = firstStr + strColor + DisplayTime(timeSpan, format, isAutoNextFormat, isMinFormatMS); });
        //Debug.LogError("timeSpan.TotalSeconds: " + timeSpan.TotalSeconds);
        if (timeSpan.TotalSeconds < 0)
        {
            if (onDone != null)
            {
                onDone.Invoke();
            }
            yield break;
        }
        yield return new WaitForSecondsRealtime(1f);
        if (isPauseApp)
        {
            yield return new WaitWhile(() => isPauseApp);
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1 - timePauseApp));
        }
        else
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
        goto Again;
    }

    IEnumerator TimeCountDown(DateTime DateTimeNext, TextMeshProUGUI labelTime, string firstStr, string strColor, string format, TextMeshProUGUI labelTimeClone = null, Action onDone = null, MyIntEvent onIntEvent = null, bool isAutoNextFormat = false, bool isMinFormatMS = false, bool isByUTC = false)
    {
        labelTime.gameObject.SetActive(true);
        DateTime DateTimeNow = isByUTC ? GetCurrentTime().ToUniversalTime() : GetCurrentTime();
        TimeSpan timeSpan;
        timeSpan = DateTimeNext.Subtract(DateTimeNow);
    Again:
        if (onIntEvent != null) onIntEvent.Invoke((int)timeSpan.TotalSeconds);
        //if (!labelTime.gameObject.activeInHierarchy)
        //{
        //    yield return new WaitForSecondsRealtime(0.1f);//child bảng thưởng lúc đầu labelTime off nên cần delay tránh trường hợp mở bảng thưởng lần đầu có mission đã hoàn thành không hiển thị nút collect
        //    if (!labelTime.gameObject.activeInHierarchy)
        //        yield break;
        //}
        labelTime.text = firstStr + strColor + DisplayTime(timeSpan, format, isAutoNextFormat, isMinFormatMS);
        if (labelTimeClone != null) labelTimeClone.text = labelTime.text;
        //Debug.LogError("timeSpan.TotalSeconds: " + timeSpan.TotalSeconds);
        if (timeSpan.TotalSeconds < 0)
        {
            if (onDone != null)
            {
                onDone.Invoke();
            }
            yield break;
        }
        yield return new WaitForSecondsRealtime(1f);
        if (isPauseApp)
        {
            yield return new WaitWhile(() => isPauseApp);
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1 - timePauseApp));
        }
        else
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
        goto Again;
    }

    IEnumerator TimeCountDown(DateTime DateTimeNext, Text labelTime, string firstStr, string strColor, string format, Text labelTimeClone = null, Action onDone = null, MyIntEvent onIntEvent = null, bool isAutoNextFormat = false, bool isMinFormatMS = false, bool isByUTC = false)
    {
        labelTime.gameObject.SetActive(true);
        DateTime DateTimeNow = isByUTC ? GetCurrentTime().ToUniversalTime() : GetCurrentTime();
        TimeSpan timeSpan;
        timeSpan = DateTimeNext.Subtract(DateTimeNow);

    //Debug.Log("DateTimeNow: " + DateTimeNow.ToString(this.format));
    //Debug.Log("DateTimeNext: " + DateTimeNext.ToString(this.format));
    //Debug.Log("TotalSeconds: " + (int)timeSpan.TotalSeconds);
    Again:
        if (onIntEvent != null) onIntEvent.Invoke((int)timeSpan.TotalSeconds);
        //if (!labelTime.gameObject.activeInHierarchy)
        //{
        //    yield return new WaitForSecondsRealtime(0.1f);//child bảng thưởng lúc đầu labelTime off nên cần delay tránh trường hợp mở bảng thưởng lần đầu có mission đã hoàn thành không hiển thị nút collect
        //    if (!labelTime.gameObject.activeInHierarchy)
        //        yield break;
        //}
        labelTime.text = firstStr + strColor + DisplayTime(timeSpan, format, isAutoNextFormat, isMinFormatMS);
        if (labelTimeClone != null) labelTimeClone.text = labelTime.text;
        //Debug.LogError("timeSpan.TotalSeconds: " + timeSpan.TotalSeconds);
        if (timeSpan.TotalSeconds < 0)
        {
            if (onDone != null)
            {
                onDone.Invoke();
            }
            yield break;
        }
        yield return new WaitForSecondsRealtime(1f);
        if (isPauseApp)
        {
            yield return new WaitWhile(() => isPauseApp);
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1 - timePauseApp));
        }
        else
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
        goto Again;
    }

    //IEnumerator TimeCountDown(DateTime DateTimeNext, UILabel labelTime, string firstStr, string strColor, string format, UILabel labelTimeClone = null, Action onDone = null, MyIntEvent onIntEvent = null)
    //{
    //    labelTime.gameObject.SetActive(true);
    //    DateTime DateTimeNow = GetCurrentTime();
    //    TimeSpan timeSpan;
    //    timeSpan = DateTimeNext.Subtract(DateTimeNow);
    //Again:
    //    if (onIntEvent != null) onIntEvent.Invoke((int)timeSpan.TotalSeconds);
    //    //if (!labelTime.gameObject.activeInHierarchy)
    //    //{
    //    //    yield return new WaitForSecondsRealtime(0.1f);//child bảng thưởng lúc đầu labelTime off nên cần delay tránh trường hợp mở bảng thưởng lần đầu có mission đã hoàn thành không hiển thị nút collect
    //    //    if (!labelTime.gameObject.activeInHierarchy)
    //    //        yield break;
    //    //}
    //    labelTime.text = firstStr + strColor + DisplayTime(timeSpan, format);
    //    if (labelTimeClone != null) labelTimeClone.text = labelTime.text;
    //    //Debug.LogError("timeSpan.TotalSeconds: " + timeSpan.TotalSeconds);
    //    if (timeSpan.TotalSeconds < 0)
    //    {
    //        TA.Extension.DebugLogEditor("Finish!!");
    //        if (onDone != null)
    //        {
    //            TA.Extension.DebugLogEditor("Xử lý Finish!!");
    //            onDone.Invoke();
    //        }
    //        yield break;
    //    }
    //    yield return new WaitForSecondsRealtime(1f);
    //    timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
    //    goto Again;
    //}

    public void CountDownTimeToNextDay(TextMeshProUGUI labelTime, string firstStr, string strColor, TextMeshProUGUI labelTimeClone = null, UnityEvent onNextDay = null, bool isAutoNextFormat = false, bool isByUTC = false)
    {
        StartCoroutine(TimeCountDownNextDay(labelTime, firstStr, strColor, labelTimeClone, onNextDay, isAutoNextFormat, isByUTC));
    }

    IEnumerator TimeCountDownNextDay(TextMeshProUGUI labelTime, string firstStr, string strColor, TextMeshProUGUI labelTimeClone = null, UnityEvent onNextDay = null, bool isAutoNextFormat = false, bool isByUTC = false)
    {
    NextCheck:
        labelTime.gameObject.SetActive(true);
        DateTime DateTimeNow = isByUTC ? GetCurrentTime().ToUniversalTime() : GetCurrentTime();
        //DateTimeNow = DateTimeNow.Add(TimeSpan.FromMinutes(363));
        DateTime DateTimeNext = new DateTime(DateTimeNow.Year, DateTimeNow.Month, DateTimeNow.Day + 1);
        TimeSpan timeSpan;
        timeSpan = DateTimeNext.Subtract(DateTimeNow);
    Again:
        if (!labelTime.gameObject.activeInHierarchy) yield break;
        labelTime.text = firstStr + strColor + DisplayTime(timeSpan, null, isAutoNextFormat);
        if (labelTimeClone != null) labelTimeClone.text = labelTime.text;
        yield return new WaitForSecondsRealtime(1f);
        if (isPauseApp)
        {
            yield return new WaitWhile(() => isPauseApp);
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1 - timePauseApp));
        }
        else
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
        //Debug.Log(timeSpan.TotalSeconds);
        if (timeSpan.TotalSeconds < 0)
        {
            Debug.Log("New Day!!");
            if (onNextDay != null) onNextDay.Invoke();
            goto NextCheck;
        }
        goto Again;
    }

    public void CountDownTimeToNextWeek(TextMeshProUGUI labelTime, string firstStr, string strColor, TextMeshProUGUI labelTimeClone = null, UnityEvent onNextWeek = null, bool isByUTC = false)
    {
        StartCoroutine(TimeCountDownNextWeek(labelTime, firstStr, strColor, labelTimeClone, onNextWeek, isByUTC));
    }

    IEnumerator TimeCountDownNextWeek(TextMeshProUGUI labelTime, string firstStr, string strColor, TextMeshProUGUI labelTimeClone = null, UnityEvent onNextWeek = null, bool isByUTC = false)
    {
    NextCheck:
        labelTime.gameObject.SetActive(true);
        DateTime DateTimeNow = isByUTC ? GetCurrentTime().ToUniversalTime() : GetCurrentTime();
        DateTime DateTimeNext = DateTimeNow;

        do
        {
            DateTimeNext = new DateTime(DateTimeNext.Year, DateTimeNext.Month, DateTimeNext.Day + 1);
        }
        while ((int)DateTimeNext.DayOfWeek != 1);

        TimeSpan timeSpan;
        timeSpan = DateTimeNext.Subtract(DateTimeNow);
    Again:
        if (!labelTime.gameObject.activeInHierarchy) yield break;
        labelTime.text = firstStr + strColor + DisplayTime(timeSpan, (timeSpan.TotalSeconds < 86400) ? null : formatTimeSpan, false);
        if (labelTimeClone != null) labelTimeClone.text = labelTime.text;
        yield return new WaitForSecondsRealtime(1f);
        if (isPauseApp)
        {
            yield return new WaitWhile(() => isPauseApp);
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1 - timePauseApp));
        }
        else
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
        if (timeSpan.TotalSeconds < 0)
        {
            Debug.Log("New Week!!");
            if (onNextWeek != null) onNextWeek.Invoke();
            goto NextCheck;
        }
        goto Again;
    }

    /// <summary>
    /// Note: Nếu so sánh thời gian kết thúc giữa creatTime và currentTime thì totalSecToFinish > 0.
    /// <br>Nếu so sánh thời gian kết thúc giữa endTime và currentTime thì totalSecToFinish = 0.</br>
    /// </summary>
    /// <param name="time"></param>
    /// <param name="totalSecToFinish"></param>
    /// <returns>true when (currentTime - time) >= totalSecToFinish.</returns>
    public bool CheckFinishTime(DateTime time, int totalSecToFinish)
    {
        TimeSpan timeSpan;
        DateTime timeCurrent = GetCurrentTime();
        timeSpan = timeCurrent.Subtract(time);
        if (timeSpan.TotalSeconds >= totalSecToFinish)
        {
            return true;
        }
        return false;
    }

    #region Âm Lịch
    //enum Can { Giáp, Ất, Bính, Đinh, Mậu, Kỷ, Canh, Tân, Nhâm, Quý }
    //enum Chi { Tý, Sửu, Dần, Mão, Thìn, Tỵ, Ngọ, Mùi, Thân, Dậu, Tuất, Hợi }

    //public string CanChiNam(int yy)
    //{
    //    return (Can)((yy + 6) % 10) + " " + (Chi)((yy + 8) % 12) + " (" + yy + ")";
    //    //Trả về kết quả vd: Tân Mùi (1991)
    //}

    //public string CanChiThang(int mm, int yy) //mm, yy = tháng, năm âm lịch
    //{
    //    return (Can)((yy * 12 + mm + 3) % 10) + " " + (Chi)((mm + 1) % 12);
    //}

    public int INT(double d)
    {
        return (int)Math.Floor(d);
    }

    public int MOD(int x, int y)
    {
        int z = x - (int)(y * Math.Floor(((double)x / y)));
        if (z == 0)
        {
            z = y;
        }
        return z;
    }

    //Đổi ngày dương lịch ra số ngày Julius
    public double SolarToJD(int D, int M, int Y)
    {
        double JD;
        if (Y > 1582 || (Y == 1582 && M > 10) || (Y == 1582 && M == 10 && D > 14))
        {
            JD = 367 * Y - INT(7 * (Y + INT((M + 9) / 12)) / 4) - INT(3 * (INT((Y + (M - 9) / 7) / 100) + 1) / 4) + INT(275 * M / 9) + D + 1721028.5;
        }
        else
        {
            JD = 367 * Y - INT(7 * (Y + 5001 + INT((M - 9) / 7)) / 4) + INT(275 * M / 9) + D + 1729776.5;
        }
        return JD;
    }

    //Đổi số ngày Julius ra ngày dương lịch
    public int[] SolarFromJD(double JD)
    {
        int Z, A, alpha, B, C, D, E, dd, mm, yyyy;
        double F;
        Z = INT(JD + 0.5);
        F = (JD + 0.5) - Z;
        if (Z < 2299161)
        {
            A = Z;
        }
        else
        {
            alpha = INT((Z - 1867216.25) / 36524.25);
            A = Z + 1 + alpha - INT(alpha / 4);
        }
        B = A + 1524;
        C = INT((B - 122.1) / 365.25);
        D = INT(365.25 * C);
        E = INT((B - D) / 30.6001);
        dd = INT(B - D - INT(30.6001 * E) + F);
        if (E < 14)
        {
            mm = E - 1;
        }
        else
        {
            mm = E - 13;
        }
        if (mm < 3)
        {
            yyyy = C - 4715;
        }
        else
        {
            yyyy = C - 4716;
        }
        return new int[] { dd, mm, yyyy };
    }

    //Chuyển đổi số ngày Julius / ngày dương lịch theo giờ địa phương LOCAL_TIMEZONE, Việt Nam: LOCAL_TIMEZONE = 7.0
    public int[] LocalFromJD(double JD)
    {
        return SolarFromJD(JD + (7.0 / 24.0));
    }

    public double LocalToJD(int D, int M, int Y)
    {
        return SolarToJD(D, M, Y) - (7.0 / 24.0);
    }

    //Tính thời điểm Sóc
    public double NewMoon(int k)
    {
        double T = k / 1236.85;
        double T2 = T * T;
        double T3 = T2 * T;
        double dr = Math.PI / 180;
        double Jd1 = 2415020.75933 + 29.53058868 * k + 0.0001178 * T2 - 0.000000155 * T3;
        Jd1 = Jd1 + 0.00033 * Math.Sin((166.56 + 132.87 * T - 0.009173 * T2) * dr);
        double M = 359.2242 + 29.10535608 * k - 0.0000333 * T2 - 0.00000347 * T3;
        double Mpr = 306.0253 + 385.81691806 * k + 0.0107306 * T2 + 0.00001236 * T3;
        double F = 21.2964 + 390.67050646 * k - 0.0016528 * T2 - 0.00000239 * T3;
        double C1 = (0.1734 - 0.000393 * T) * Math.Sin(M * dr) + 0.0021 * Math.Sin(2 * dr * M);
        C1 = C1 - 0.4068 * Math.Sin(Mpr * dr) + 0.0161 * Math.Sin(dr * 2 * Mpr);
        C1 = C1 - 0.0004 * Math.Sin(dr * 3 * Mpr);
        C1 = C1 + 0.0104 * Math.Sin(dr * 2 * F) - 0.0051 * Math.Sin(dr * (M + Mpr));
        C1 = C1 - 0.0074 * Math.Sin(dr * (M - Mpr)) + 0.0004 * Math.Sin(dr * (2 * F + M));
        C1 = C1 - 0.0004 * Math.Sin(dr * (2 * F - M)) - 0.0006 * Math.Sin(dr * (2 * F + Mpr));
        C1 = C1 + 0.0010 * Math.Sin(dr * (2 * F - Mpr)) + 0.0005 * Math.Sin(dr * (2 * Mpr + M));
        double deltat;
        if (T < -11)
        {
            deltat = 0.001 + 0.000839 * T + 0.0002261 * T2 - 0.00000845 * T3 - 0.000000081 * T * T3;
        }
        else
        {
            deltat = -0.000278 + 0.000265 * T + 0.000262 * T2;
        };
        double JdNew = Jd1 + C1 - deltat;
        return JdNew;
    }

    //Tính vị trí của mặt trời
    public double SunLongitude(double jdn)
    {
        double T = (jdn - 2451545.0) / 36525;
        double T2 = T * T;
        double dr = Math.PI / 180;
        double M = 357.52910 + 35999.05030 * T - 0.0001559 * T2 - 0.00000048 * T * T2;
        double L0 = 280.46645 + 36000.76983 * T + 0.0003032 * T2;
        double DL = (1.914600 - 0.004817 * T - 0.000014 * T2) * Math.Sin(dr * M);
        DL = DL + (0.019993 - 0.000101 * T) * Math.Sin(dr * 2 * M) + 0.000290 * Math.Sin(dr * 3 * M);
        double L = L0 + DL;
        L = L * dr;
        L = L - Math.PI * 2 * (INT(L / (Math.PI * 2)));
        return L;
    }

    //Tính tháng âm lịch chứa ngày Đông chí
    public int[] LunarMonth11(int Y)
    {
        double off = LocalToJD(31, 12, Y) - 2415021.076998695;
        int k = INT(off / 29.530588853);
        double jd = NewMoon(k);
        int[] ret = LocalFromJD(jd);
        double sunLong = SunLongitude(LocalToJD(ret[0], ret[1], ret[2]));
        if (sunLong > 3 * Math.PI / 2)
        {
            jd = NewMoon(k - 1);
        }
        return LocalFromJD(jd);
    }

    /// <summary>
    /// Tính năm âm lịch
    /// </summary>
    /// <param name="Y"></param>
    /// <returns></returns>
    public int[][] LunarYear(int Y)
    {
        int[][] ret = null;
        int[] month11A = LunarMonth11(Y - 1);
        double jdMonth11A = LocalToJD(month11A[0], month11A[1], month11A[2]);
        int k = (int)Math.Floor(0.5 + (jdMonth11A - 2415021.076998695) / 29.530588853);
        int[] month11B = LunarMonth11(Y);
        double off = LocalToJD(month11B[0], month11B[1], month11B[2]) - jdMonth11A;
        bool leap = off > 365.0;
        if (!leap)
        {
            ret = new int[13][];
        }
        else
        {
            ret = new int[14][];
        }
        ret[0] = new int[] { month11A[0], month11A[1], month11A[2], 0, 0 };
        ret[ret.Length - 1] = new int[] { month11B[0], month11B[1], month11B[2], 0, 0 };
        for (int i = 1; i < ret.Length - 1; i++)
        {
            double nm = NewMoon(k + i);
            int[] a = LocalFromJD(nm);
            ret[i] = new int[] { a[0], a[1], a[2], 0, 0 };
        }
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i][3] = MOD(i + 11, 12);
        }
        if (leap)
        {
            initLeapYear(ret);
        }
        return ret;
    }

    /// <summary>
    /// Tính tháng nhuận
    /// </summary>
    /// <param name="ret"></param>
    public void initLeapYear(int[][] ret)
    {
        double[] sunLongitudes = new double[ret.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            int[] a = ret[i];
            double jdAtMonthBegin = LocalToJD(a[0], a[1], a[2]);
            sunLongitudes[i] = SunLongitude(jdAtMonthBegin);
        }
        bool found = false;
        for (int i = 0; i < ret.Length; i++)
        {
            if (found)
            {
                ret[i][3] = MOD(i + 10, 12);
                continue;
            }
            double sl1 = sunLongitudes[i];
            double sl2 = sunLongitudes[i + 1];
            bool hasMajorTerm = Math.Floor(sl1 / Math.PI * 6) != Math.Floor(sl2 / Math.PI * 6);
            if (!hasMajorTerm)
            {
                found = true;
                ret[i][4] = 1;
                ret[i][3] = MOD(i + 10, 12);
            }
        }
    }
    /// <summary>
    /// Đổi ngày dương lịch ra âm lịch
    /// </summary>
    /// <param name="D">ngày dương lịch</param>
    /// <param name="M">tháng dương lịch</param>
    /// <param name="Y">năm dương lịch</param>
    /// <returns>{ dd, mm, yy, ly[i][4] } Nếu ly[i][4] == 1 => tháng mm Nhuận</returns>
    public int[] Solar2Lunar(int D, int M, int Y)
    {
        int yy = Y;
        int[][] ly = LunarYear(Y);
        int[] month11 = ly[ly.Length - 1];
        double jdToday = LocalToJD(D, M, Y);
        double jdMonth11 = LocalToJD(month11[0], month11[1], month11[2]);
        if (jdToday >= jdMonth11)
        {
            ly = LunarYear(Y + 1);
            yy = Y + 1;
        }
        int i = ly.Length - 1;
        while (jdToday < LocalToJD(ly[i][0], ly[i][1], ly[i][2]))
        {
            i--;
        }
        int dd = (int)(jdToday - LocalToJD(ly[i][0], ly[i][1], ly[i][2])) + 1;
        int mm = ly[i][3];
        if (mm >= 11)
        {
            yy--;
        }
        return new int[] { dd, mm, yy, ly[i][4] };
        //Nếu ly[i][4] == 1 => tháng mm Nhuận
    }
    #endregion
}

/*
        DateTime DateTimeNow = DateTime.Now;
        DateTime TimeBuyRemoveADS;
        TimeSpan interval;
        TimeBuyRemoveADS = DateTime.ParseExact(PlayerPrefs.GetString("TimeBuyRemoveADS14D"), format, null);
        interval = DateTimeNow.Subtract(TimeBuyRemoveADS);
*/
