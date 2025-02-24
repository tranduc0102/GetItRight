using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Game;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Transform> players;
    [SerializeField] private Vector3 positionTarget;
    [SerializeField] private float speed;
    [SerializeField] private AnimFace _animFace;
    private Vector3[] initialPositions;
    private int currentIndex = 0;
    private List<float> durations = new List<float>();

    private void OnValidate()
    {
        initialPositions = new Vector3[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            initialPositions[i] = players[i].position;
            durations.Add(Vector3.Distance(players[i].localPosition, positionTarget) / speed);
        }
    }
    public void SetNumberPlayer(int number, bool active)
    {
        if (!active)
        {
            for (int i = players.Count - 1; i >= players.Count - number; i--)
            {
                players[i].gameObject.SetActive(false);
            }   
        }
        else
        {
            for (int i = 0; i < number; i++)
            {
                players[i].gameObject.SetActive(true);
            }  
        }
    }

    public void MoveToTarget()
    {
        if (currentIndex >= players.Count)
        {
            currentIndex = 0;
        }

        Transform player = players[currentIndex];

        player.DORotate(new Vector3(-15.354f, 180, 0), 1f)
              .OnComplete(() =>
              {
                  player.DOLocalMove(positionTarget, durations[currentIndex]).OnComplete(() =>
                  {
                      GameController.Instance.playerMoved = false;
                  });
              });
    }

    public void NextPlayerMovement()
    {
        Transform player = players[currentIndex];

        player.DORotate(new Vector3(0, 360, 0), 1f)
              .OnComplete(() =>
              {
                  player.DOMove(initialPositions[currentIndex], durations[currentIndex])
                        .OnComplete(() =>
                        {
                            currentIndex++;
                            MoveToTarget();
                        });
              });
    }
    public void PlayAnim(string animName)
    {
        Animator anim = players[currentIndex].GetComponent<Animator>();
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            anim.SetTrigger(animName);
            StartCoroutine(WaitForAnimationStart(anim, animName));
        }
    }

    private IEnumerator WaitForAnimationStart(Animator anim, string animName)
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(animName));
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        if (Enum.TryParse(animName, out StateFace state))
        {
            _animFace.SetState(state, animLength);
        }
    }
    public void ResetPlayers()
    {
        DOTween.KillAll();

        for (int i = 0; i < players.Count; i++)
        {
            players[i].position = initialPositions[i];
            players[i].rotation = Quaternion.Euler(0, 360, 0);
        }
        currentIndex = 0;
    }
}
