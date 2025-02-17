using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] private GameObject panelSetting;

    private bool CanMusic
    {
        get => PlayerPrefs.GetInt("CanMusic", 1) == 1;
        set => PlayerPrefs.SetInt("CanMusic", value ? 1 : 0);
    }
    private bool CanSound
    {
        get => PlayerPrefs.GetInt("CanSound", 1) == 1;
        set => PlayerPrefs.SetInt("CanSound", value ? 1 : 0);
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
        panelSetting.SetActive(enable);
    }
    
    private void LoadSettings()
    {
        _spriteIconMusic.sprite = CanMusic ? _spritesIconMusic[0] : _spritesIconMusic[1];
        _spriteIconSound.sprite = CanSound ? _spritesIconSound[0] : _spritesIconSound[1];
        _spriteIconVibration.sprite = IsOffVibration ? _spritesIconVibration[1] : _spritesIconVibration[0];
    }
    
    private void ButtonMusicClick()
    {
        CanMusic = !CanMusic;
        _spriteIconMusic.sprite = CanMusic ? _spritesIconMusic[0] : _spritesIconMusic[1];
        _spriteIconMusic.SetNativeSize();
    }
    
    private void ButtonSoundClick()
    {
        CanSound = !CanSound;
        _spriteIconSound.sprite = CanSound ? _spritesIconSound[0] : _spritesIconSound[1];
        _spriteIconSound.SetNativeSize();
    }
    
    private void ButtonVibrationClick()
    {
        IsOffVibration = !IsOffVibration;
        _spriteIconVibration.sprite = IsOffVibration ? _spritesIconVibration[1] : _spritesIconVibration[0];
        _spriteIconVibration.SetNativeSize();
    }
}