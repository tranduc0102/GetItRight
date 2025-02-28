using DG.Tweening;
using pooling;
using UnityEngine;

public class EffectRewardCoin : MonoBehaviour
{
    [SerializeField] private GameObject PileOfCoinParent;
    [SerializeField] private Vector3[] InitialPos;
    [SerializeField] private Quaternion[] InitialRot;

    void Start()
    {
        InitialPos = new Vector3[10];
        InitialRot = new Quaternion[10];
        for (int i = 0; i < PileOfCoinParent.transform.childCount; i++)
        {
            InitialPos[i] = PileOfCoinParent.transform.GetChild(i).position;
            InitialRot[i] = PileOfCoinParent.transform.GetChild(i).rotation;
        }
    }
    private void OnEnable()
    {
        RewardPileOfCoint();
    }
    private void ResetState()
    {
        PileOfCoinParent = transform.gameObject;
        for (int i = 0; i < PileOfCoinParent.transform.childCount; i++)
        {
            PileOfCoinParent.transform.GetChild(i).position = InitialPos[i];
            PileOfCoinParent.transform.GetChild(i).rotation = InitialRot[i];
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < PileOfCoinParent.transform.childCount; i++)
        {
            PileOfCoinParent.transform.GetChild(i).DOKill();
        }
    }
    private void RewardPileOfCoint()
    {
        ResetState();
        float delay = 0;
        PileOfCoinParent.SetActive(true);
        for (int i = 0; i < PileOfCoinParent.transform.childCount; i++)
        {
            PileOfCoinParent.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            PileOfCoinParent.transform.GetChild(i).DOMove(UIController.instance.imgCoin.transform.position, 1f).SetDelay(delay + 0.5f)
                            .SetEase(Ease.OutBack);

            PileOfCoinParent.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash).OnComplete( () => UIController.instance.UpdateCoin(1));

            float delay1 = delay;
            PileOfCoinParent.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 0.8f).SetEase(Ease.OutBack).OnComplete(delegate
            {
                if (Mathf.Approximately(delay1, 0.9f))
                {
                    PoolingManager.Despawn(gameObject);
                }
            });
            
            delay += 0.1f;
        }

    }
}
