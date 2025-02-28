using System;
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
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int TurnLeft = Animator.StringToHash("TurnLeft");
    [SerializeField] private List<Transform> players = new List<Transform>(3);
    [SerializeField] private Transform currentPlayer;
    [SerializeField] private List<AnimFace> _animFaces = new List<AnimFace>(3);
    [SerializeField] private Vector3[] posSpawnOtherPlayer;
    private int currentIndex = 0;
    public bool isBot;

    private void NextPlayerMovement()
    {
        Transform player = players[currentIndex];           
        int nextIndex = (currentIndex + 1) % players.Count;
        Transform nextPlayer = players[nextIndex];        
        Vector3 originalPos = player.localPosition;           
        
        Animator animator = player.GetComponentInChildren<Animator>();
        Sequence sequence = DOTween.Sequence();
        
        animator.SetTrigger(Run);
        sequence.Append(player.DOLocalRotate(Quaternion.Euler(15.35f, 0f, 0f).eulerAngles, 0.5f));
        sequence.Append(player.DOLocalMove(nextPlayer.localPosition, 1));

        sequence.Append(player.DOLocalRotate(Quaternion.Euler(15.35f, -180f, 0f).eulerAngles * -1f, 0.5f).OnComplete(delegate
        {
            animator.SetTrigger(Idle);
        }));

        Animator animator2 = nextPlayer.GetComponentInChildren<Animator>();
        animator2.SetTrigger(Run);
        nextPlayer.DOLocalMove(originalPos, 2f).OnComplete(delegate
        {
            animator2.SetTrigger(Idle);
        });

        sequence.OnComplete(() =>
        {
            currentIndex = nextIndex;
            if ((isBot && currentIndex == 0) || !isBot)
            {
                GameController.Instance.CanClick = true;
            }
        });
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void PlayAnim(StateFace animName)
    {
        Animator anim = players[currentIndex].GetComponentInChildren<Animator>();
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
                    Animator anim1 = players[i].GetComponentInChildren<Animator>();
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
                    Animator anim1 = players[i].GetComponentInChildren<Animator>();
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
        if (_animFaces[0] == null) yield break;
        _animFaces[0].SetState(animName, animLength);
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
    private void SpawnOtherPlayers()
    {
        var characters = GameController.Instance.DataCharacter.characters;
        int currentPlayer = PlayerPrefs.GetInt(USESTRING.CURRENT_PLAYER, 0);

        int random1 = GetUniqueRandomIndex(characters.Count, currentPlayer);
        int random2 = GetUniqueRandomIndex(characters.Count, currentPlayer, random1);

        SpawnCharacter(random1, posSpawnOtherPlayer[0]);
        SpawnCharacter(random2, posSpawnOtherPlayer[1]);
    }

    private int GetUniqueRandomIndex(int max, params int[] exclude)
    {
        int random;
        do
        {
            random = UnityEngine.Random.Range(0, max);
        } while (exclude.Contains(random));
        return random;
    }
    private void SpawnCharacter(int index, Vector3 spawnPos)
    {
        var characters = GameController.Instance.DataCharacter.characters;
        Transform t = PoolingManager.Spawn(characters[index], transform.position, characters[index].rotation, transform);
        t.localPosition = spawnPos;
        players.Add(t);
    
        if (t.TryGetComponent(out AnimFace animFace))
        {
            _animFaces.Add(animFace);
        }
    }
    public void ChangePlayer(Transform player, bool canChange)
    {
        if (canChange && players[0] != null)
        {
            PoolingManager.Despawn(players[0].gameObject);
        }
        players.Add(PoolingManager.Spawn(player, transform.position + player.position, player.rotation, transform));
        if (!players[0].TryGetComponent(out AnimFace _animFace))
        {
            Debug.LogWarning("Character mới không có animface");
        }
        else
        {
            _animFaces.Add(_animFace);
        }
    }
    public IEnumerator AnimRun(Action onFinish1, Action onFinish2)
    {
        currentPlayer = players[0];
        Animator anim1 = currentPlayer.GetComponentInChildren<Animator>();
        anim1.SetTrigger(TurnLeft);
        currentPlayer.DOLocalRotate(Quaternion.Euler(0f, 0f, 0f).eulerAngles, 0.5f).OnComplete(delegate
        {
            anim1.SetTrigger(Run);
            onFinish1?.Invoke();
        });
        yield return new WaitForSeconds(1.95f);
        anim1.SetTrigger(Idle);
        onFinish2?.Invoke();
        yield return new WaitForSeconds(0.5f);
        currentPlayer.DOLocalRotate(Quaternion.Euler(15.35f, -180f, 0f).eulerAngles * -1f, 1f);
    }
}
