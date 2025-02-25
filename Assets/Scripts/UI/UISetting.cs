using _Scripts;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] private GameObject panelSetting;

    public bool IsMuteMusic
    {
        get => PlayerPrefs.GetInt("IsMuteMusic", 0) == 1;
        set => PlayerPrefs.SetInt("IsMuteMusic", value ? 1 : 0);
    }
    public bool IsMuteSound
    {
        get => PlayerPrefs.GetInt("IsMuteSound", 0) == 1;
        set => PlayerPrefs.SetInt("IsMuteSound", value ? 1 : 0);
    }
    public bool IsOffVibration
    {
        get => PlayerPrefs.GetInt("IsOffVibration", 0) == 1;
        set => PlayerPrefs.SetInt("IsOffVibration", value ? 1 : 0);
    }
    
    [Header("Button Music")]
    [SerializeField] private Button _btnMusic;
    [SerializeField] private Image _spriteIconMusic;
    [SerializeField] private Sprite[] _spritesIconMusic;
    
    [Header("Button Sound")]
    [SerializeField] private Button _btnSound;
    [SerializeField] private Image _spriteIconSound;
    [SerializeField] private Sprite[] _spritesIconSound;
    
    [Header("Button Vibration")]
    [SerializeField] private Button _btnVibration;
    [SerializeField] private Image _spriteIconVibration;
    [SerializeField] private Sprite[] _spritesIconVibration;

    void Start()
    {
        _btnMusic.onClick.AddListener(ButtonMusicClick);
        _btnSound.onClick.AddListener(ButtonSoundClick);
        _btnVibration.onClick.AddListener(ButtonVibrationClick);
        
        LoadSettings();
    }
    
    public void ShowDisplay(bool enable = false)
    {
        AudioManager.instance.PlaySoundButtonClick();
        panelSetting.SetActive(enable);
    }
    
    private void LoadSettings()
    {
        _spriteIconMusic.sprite = IsMuteMusic ? _spritesIconMusic[1] : _spritesIconMusic[0];
        _spriteIconSound.sprite = IsMuteSound ? _spritesIconSound[1] : _spritesIconSound[0];
        _spriteIconVibration.sprite = IsOffVibration ? _spritesIconVibration[1] : _spritesIconVibration[0];
    }
    
    private void ButtonMusicClick()
    {
        AudioManager.instance.PlaySoundButtonClick();
        IsMuteMusic = !IsMuteMusic;
        _spriteIconMusic.sprite = IsMuteMusic ? _spritesIconMusic[1] : _spritesIconMusic[0];
        _spriteIconMusic.SetNativeSize();
        AudioManager.instance.SetMuteMusic();
    }
    
    private void ButtonSoundClick()
    {
        AudioManager.instance.PlaySoundButtonClick();
        IsMuteSound = !IsMuteSound;
        _spriteIconSound.sprite = IsMuteSound ? _spritesIconSound[1] : _spritesIconSound[0];
        _spriteIconSound.SetNativeSize();
        AudioManager.instance.SetMuteSounds();
    }
    
    private void ButtonVibrationClick()
    {
        AudioManager.instance.PlaySoundButtonClick();
        IsOffVibration = !IsOffVibration;
        _spriteIconVibration.sprite = IsOffVibration ? _spritesIconVibration[1] : _spritesIconVibration[0];
        _spriteIconVibration.SetNativeSize();
    }
}