using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public Transform playerTransform;
    public float speed;
}

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    [SerializeField] private Vector2 center = Vector2.zero;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float rotationSpeed = 50f;

    private float currentAngle;
    private bool canMove = false;
    private float targetAngle = 0f;  

    public void SetCanMove(bool canMove)
    {
        if (canMove)
        {
            targetAngle = currentAngle + 90f;            
            if (targetAngle >= 360f) targetAngle -= 360f; 
        }

        this.canMove = canMove;
    }
    public void SetNumberPlayer(int number, bool active)
    {
        if (!active)
        {
            for (int i = players.Count - 1; i >= players.Count - number; i--)
            {
                players[i].playerTransform.gameObject.SetActive(false);
            }   
        }
        else
        {
            for (int i = 0; i < number; i++)
            {
                players[i].playerTransform.gameObject.SetActive(true);
            }  
        }
    }

    private void Update()
    {
        if (canMove)
        {
            MovePlayersInCircle();
        }
    }

    private void MovePlayersInCircle()
    {
        if (players.Count == 0) return;

        float angleStep = 360f / players.Count;

        for (int i = 0; i < players.Count; i++)
        {
            float angle = currentAngle + angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            Vector3 newPosition = new Vector3(
                Mathf.Cos(radian) * radius + center.x,
                Mathf.Sin(radian) * radius + center.y,
                players[i].playerTransform.position.z
            );

            players[i].playerTransform.position = newPosition;
        }

        float step = rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step);

        if (Mathf.Abs(currentAngle - targetAngle) < 1f)
        {
            canMove = false; 
        }
    }
}