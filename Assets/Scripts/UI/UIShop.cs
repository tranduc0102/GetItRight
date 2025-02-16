using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private Button btnTheme;
    [SerializeField] private GameObject viewTheme;
    
    [SerializeField] private Button btnSkin;
    [SerializeField] private GameObject viewSkin;

    [SerializeField] private Sprite[] sprites;
    void Start()
    {
        var imgTheme = btnTheme.GetComponent<Image>();
        var imgSkin = btnSkin.GetComponent<Image>();
        btnTheme.onClick.AddListener(delegate
        {
            imgTheme.sprite = sprites[0];
            imgSkin.sprite = sprites[1];
            btnTheme.transform.DOScale(Vector3.one, 0.2f);
            btnSkin.transform.DOScale(Vector3.one * 0.9f, 0.2f);
            viewSkin.SetActive(false);
            viewTheme.SetActive(true);
            btnTheme.transform.SetSiblingIndex(1);
            btnSkin.transform.SetSiblingIndex(0);
        });
        btnSkin.onClick.AddListener(delegate
        {
            imgSkin.sprite = sprites[0];
            imgTheme.sprite = sprites[1];
            btnSkin.transform.DOScale(Vector3.one, 0.2f);
            btnTheme.transform.DOScale(Vector3.one * 0.9f, 0.2f);
            viewSkin.SetActive(true);
            viewTheme.SetActive(false);
            btnSkin.transform.SetSiblingIndex(1);
            btnTheme.transform.SetSiblingIndex(0);
        });
    }
    
}