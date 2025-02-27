using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEngine;
using DG.Tweening;
using Game;
using pooling;

public class PlayerManager : MonoBehaviour
{
    private static readonly int DoanDung = Animator.StringToHash("DoanDung");
    private static readonly int Win = Animator.StringToHash("Win");
    [SerializeField] private List<Transform> players;
    [SerializeField] private float speed;
    [SerializeField] private AnimFace[] _animFace;
    [SerializeField] private Vector3[] posSpawnOtherPlayer;
    private int currentIndex = 0;
    public bool isBot;

   
    private void NextPlayerMovement()
    {
        Transform player = players[currentIndex];           
        int nextIndex = (currentIndex + 1) % players.Count;
        Transform nextPlayer = players[nextIndex];        
        Vector3 originalPos = player.localPosition;           

        Sequence sequence = DOTween.Sequence();

        sequence.Append(player.DOLocalRotate(Quaternion.Euler(15.35f, 0f, 0f).eulerAngles, 0.5f));

        sequence.Append(player.DOLocalMove(nextPlayer.localPosition, 1));

        sequence.Append(player.DOLocalRotate(Quaternion.Euler(15.35f, -180f, 0f).eulerAngles * -1f, 0.5f));

        nextPlayer.DOLocalMove(originalPos, 2f);

        sequence.OnComplete(() =>
        {
            currentIndex = nextIndex;
            if ((isBot && currentIndex == 0) || !isBot)
            {
                GameController.Instance.CanClick = true;
            }
            else
            {
                StartCoroutine(BotClick());
            }
        });
    }
    private IEnumerator BotClick()
    {
        int index = GameController.Instance.Board.CurrentIndex % GameController.Instance.CurrentLevelGame.amountBox;
        while (index < GameController.Instance.CurrentLevelGame.amountBox)
        {
            GameController.Instance.PanelAnswerController.BotClickRandom();
            index++;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void PlayAnim(StateFace animName)
    {
        Animator anim = players[currentIndex].GetComponent<Animator>();
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(animName.ToString()))
        {
            anim.SetTrigger(animName.ToString());
            StartCoroutine(WaitForAnimationStart(currentIndex, anim, animName));
        }
        if (players.Count > 1 && animName == StateFace.DoanSai)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (i != currentIndex)
                {
                    Animator anim1 = players[i].GetComponent<Animator>();
                    anim1.SetTrigger(DoanDung);
                    StartCoroutine(WaitForAnimationStart(i, anim1, StateFace.Smile, true));
                }
            }
            DOVirtual.DelayedCall(1f + Time.deltaTime, () => GameController.Instance.CanClick = false);
            DOVirtual.DelayedCall(2f, NextPlayerMovement);
        }else if (players.Count > 1 && animName == StateFace.ThatBai)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (i != currentIndex)
                {
                    Animator anim1 = players[i].GetComponent<Animator>();
                    anim1.SetTrigger(Win);
                    StartCoroutine(WaitForAnimationStart(i, anim1, StateFace.Win));
                }
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator WaitForAnimationStart(int index, Animator anim, StateFace animName, bool isSmile = false)
    {
        if(!isSmile) yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(animName.ToString()));
        else yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("DoanDung"));
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        if (animName == StateFace.DoanSai)
        {
            AudioManager.instance.PlaySoundAngry();
        }
        if (_animFace[index] == null) yield break;
        _animFace[index].SetState(animName, animLength);
        Debug.Log(animName.ToString());
    }
    public void ResetPlayers()
    {
        currentIndex = 0;
        foreach (var player in players.ToList())
        {
            if (player != players[currentIndex])
            {
                PoolingManager.Despawn(player.gameObject);
                players.Remove(player);
            }
        }
    }
    public void SpawnOtherPlayer()
    {
        
    }
    public void ChangePlayer(Transform player)
    {
        PoolingManager.Despawn(players[0].gameObject);
        players[0] = PoolingManager.Spawn(player, transform.position + player.position, player.rotation, transform);
        if (!players[0].gameObject.TryGetComponent(out _animFace))
        {
            Debug.LogWarning("Character mới không có animface");
        }    
    }
}
