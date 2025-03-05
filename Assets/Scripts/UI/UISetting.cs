using _Scripts;
using _Scripts.Extension;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] private UIAppear dime;
    [SerializeField] private UIAppear popup;

    public bool IsMuteMusic
    {
        get => PlayerPrefs.GetInt("IsMuteMusic", 0) == 1;
        set
        {
            if (value == false)
            {
                PlayerPrefs.SetInt("IsMuteMusic", 0);
            }
            else
            {
                PlayerPrefs.SetInt("IsMuteMusic", 1);
            }
        }    
    }
    public bool IsMuteSound
    {
        get => PlayerPrefs.GetInt("IsMuteSound", 0) == 1;
        set
        {
            if (value == false)
            {
                PlayerPrefs.SetInt("IsMuteSound", 0);
            }
            else
            {
                PlayerPrefs.SetInt("IsMuteSound", 1);
            }
        }
    }
    public bool IsOffVibration
    {
        get => PlayerPrefs.GetInt("IsOffVibration", 0) == 1;
        set
        {
            if (value == false)
            {
                PlayerPrefs.SetInt("IsOffVibration", 0);
            }
            else
            {
                PlayerPrefs.SetInt("IsOffVibration", 1);
            }
        } 
    }
    
    [Header("Toggle")]
    [SerializeField] private Toggle toggleSound;
    [SerializeField] private Toggle toggleMusic;
    [SerializeField] private Toggle toggleVibrate;

    private void Awake()
    {
        DOTween.SetTweensCapacity(500, 500);

        if (IsMuteMusic)
        {
            toggleMusic.isOn = false;
        }
        else
        {
            toggleMusic.isOn = true;
        }

        if (IsMuteSound)
        {
            toggleSound.isOn = false;
        }
        else
        {
            toggleSound.isOn = true;
        }

        if (IsOffVibration)
        {
            toggleVibrate.isOn = false;
        }
        else
        {
            toggleVibrate.isOn = true;
        }
    }

    public void DisplaySetting(bool enable)
    {
        if (enable)
        {
            dime.gameObject.SetActive(true);
            popup.gameObject.SetActive(true);
        }
        else
        {
            popup.Close(delegate
            {
                dime._Close(true);
            },true);
        }
    }
    
    public void ButtonMusicClick()
    {
        if (IsMuteMusic == toggleMusic.isOn)
        {
            IsMuteMusic = !IsMuteMusic;
            AudioManager.instance.SetMuteMusic();
        }
    }
    
    public void ButtonSoundClick()
    {
        if (IsMuteSound == toggleSound.isOn)
        {
            IsMuteSound = !IsMuteSound;
            AudioManager.instance.SetMuteSounds();
        }

    }
    
    public void ButtonVibrationClick()
    {
        if (IsOffVibration == toggleVibrate.isOn)
        {
            IsOffVibration = !IsOffVibration;
        }
    }
}