using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private List<Player> players = new List<Player>(3);
    [SerializeField] private Player currentPlayer;
    [SerializeField] private Vector3[] posSpawnOtherPlayer;
    private int currentIndex = 0;

    private void NextPlayerMovement()
    {
        Player player = players[currentIndex];           
        int nextIndex = (currentIndex + 1) % players.Count;
        Player nextPlayer = players[nextIndex];        
        Vector3 originalPos = player.transform.localPosition;           
        
        Animator animator = player.GetComponentInChildren<Animator>();
        Sequence sequence = DOTween.Sequence();
        
        player.PlayAnimation(Run);
        sequence.Append(player.transform.DOLocalRotate(Quaternion.Euler(15.35f, 0f, 0f).eulerAngles, 0.5f));
        sequence.Append(player.transform.DOLocalMove(nextPlayer.transform.localPosition, 1));

        sequence.Append(player.transform.DOLocalRotate(Quaternion.Euler(15.35f, -180f, 0f).eulerAngles * -1f, 0.5f).OnComplete(delegate
        {
            player.PlayAnimation(Run);
        }));
       
        nextPlayer.PlayAnimation(Run);
        nextPlayer.transform.DOLocalMove(originalPos, 2f).OnComplete(delegate
        {
            nextPlayer.PlayAnimation(Idle);
        });

        sequence.OnComplete(() =>
        {
            currentIndex = nextIndex;
        });
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void PlayAnim(StateFace animName)
    {
        players[currentIndex].PlayAnimWithFace(animName);
        if (players.Count > 1 && animName == StateFace.DoanSai)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (i != currentIndex)
                {
                    players[i].PlayAnimWithFace(StateFace.Smile, true);
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
                    players[i].PlayAnimWithFace(StateFace.Win);
                }
            }
        }
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
    public void SpawnOtherPlayers()
    {
        ResetPlayers();
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
        Player t = PoolingManager.Spawn(characters[index], transform.position, characters[index].transform.rotation, transform);
        t.transform.localPosition = spawnPos;
        players.Add(t);
    }
    public void ChangePlayer(Player player, bool canChange)
    {
        if (canChange && players[0] != null)
        {
            PoolingManager.Despawn(players[0].gameObject);
        }
        players.Add(PoolingManager.Spawn(player, transform.position + player.transform.position, player.transform.rotation, transform));
    }
    public IEnumerator AnimRun(Action onFinish1, Action onFinish2)
    {
        currentPlayer = players[0];
     
        currentPlayer.PlayAnimation(TurnLeft);
        currentPlayer.transform.DOLocalRotate(Quaternion.Euler(0f, 0f, 0f).eulerAngles, 0.5f).OnComplete(delegate
        {
            currentPlayer.PlayAnimation(Run);
            onFinish1?.Invoke();
        });
        yield return new WaitForSeconds(2.15f);
        currentPlayer.PlayAnimation(Idle);
        onFinish2?.Invoke();
        yield return new WaitForSeconds(0.5f);
        currentPlayer.transform.DOLocalRotate(Quaternion.Euler(15.35f, -180f, 0f).eulerAngles * -1f, 1f);
    }
}
