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

    private void MovePlayersInCircle()
    {
        
    }
}