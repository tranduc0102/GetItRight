using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;
using DG.Tweening;
using Game;
using pooling;

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

        /*player.DORotate(new Vector3(-15.354f, 180, 0), 1f)
              .OnComplete(() =>
              {
                  player.DOLocalMove(positionTarget, durations[currentIndex]).OnComplete(() =>
                  {
                      GameController.Instance.playerMoved = false;
                  });
              });*/
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
    public void PlayAnim(StateFace animName)
    {
        Animator anim = players[currentIndex].GetComponent<Animator>();
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(animName.ToString()))
        {
            anim.SetTrigger(animName.ToString());
            StartCoroutine(WaitForAnimationStart(anim, animName));
        }
    }

    private IEnumerator WaitForAnimationStart(Animator anim, StateFace animName)
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(animName.ToString()));
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        if (animName == StateFace.DoanSai)
        {
            AudioManager.instance.PlaySoundAngry();
        }
        if (!_animFace) yield break;
        _animFace.SetState(animName, animLength);
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
    public Transform player1;
    public Transform player2;
    public Transform player3;
    public Transform player4;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangePlayer(player1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangePlayer(player2);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangePlayer(player3);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangePlayer(player4);
        }
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
