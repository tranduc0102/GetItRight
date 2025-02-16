using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] private GameObject panelSetting;

    private bool CanMusic
    {
        get => PlayerPrefs.GetInt("CanMusic", 0) == 1;
        set => PlayerPrefs.SetInt("CanMusic", value ? 1 : 0);
    }
    private bool CanSound
    {
        get => PlayerPrefs.GetInt("CanSound", 0) == 1;
        set => PlayerPrefs.SetInt("CanSound", value ? 1 : 0);
    }
    private bool CanVibration
    {
        get => PlayerPrefs.GetInt("CanVibration", 0) == 1;
        set => PlayerPrefs.SetInt("CanVibration", value ? 1 : 0);
    }
    
    [Space]
    [Header("Button Music")]
    [SerializeField] private Button _btnMusic;
    [SerializeField] private Image _spriteIconMusic;
    [SerializeField] private Sprite[] _spritesIconMusic;
    
    [Space]
    [Header("Button Sound")]
    [SerializeField] private Button _btnSound;
    [SerializeField] private Image _spriteIconSound;
    [SerializeField] private Sprite[] _spritesIconSound;
    
    [Space]
    [Header("Button Vibration")]
    [SerializeField] private Button _btnVibration;
    [SerializeField] private Image _spriteIconVibration;
    [SerializeField] private Sprite[] _spritesIconVibration;
    void Start()
    {
        _btnMusic.onClick.AddListener(ButtonMusicClick);
        _btnSound.onClick.AddListener(ButtonSoundClick);
        _btnVibration.onClick.AddListener(ButtonVibrationClick);
    }
    public void ShowDisplay(bool enable = false)
    {
        panelSetting.SetActive(enable);
    }
    private void LoadSettings()
    {
        
    }
    private void ButtonMusicClick()
    {
        if (_spriteIconMusic.sprite == _spritesIconMusic[0])
        {
            _spriteIconMusic.sprite = _spritesIconMusic[1];
            CanMusic = false;
        }
        else
        {
            _spriteIconMusic.sprite = _spritesIconMusic[0];
            CanMusic = true;
        }
        _spriteIconMusic.SetNativeSize();
    }
    private void ButtonSoundClick()
    {
        if (_spriteIconSound.sprite == _spritesIconSound[0])
        {
            _spriteIconSound.sprite = _spritesIconSound[1];
            CanSound = false;
        }
        else
        {
            _spriteIconSound.sprite = _spritesIconSound[0];
            CanSound = true;
        }
        _spriteIconSound.SetNativeSize();
    }
    private void ButtonVibrationClick()
    {
        if (_spriteIconVibration.sprite == _spritesIconVibration[0])
        {
            _spriteIconVibration.sprite = _spritesIconVibration[1];
            CanVibration = false;
        }
        else
        {
            _spriteIconVibration.sprite = _spritesIconVibration[0];
            CanVibration = true;
        }
        _spriteIconVibration.SetNativeSize();
    }
    
}
