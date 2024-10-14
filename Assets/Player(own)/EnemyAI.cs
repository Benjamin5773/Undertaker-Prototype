using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    // patroling
    public UnityEngine.Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointrange;

    //Attacking
    
}
