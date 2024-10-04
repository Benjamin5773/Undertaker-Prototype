using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject player;

    [Header("Basic")]
    [SerializeField] float bossMaxHealth = 5f;
    [SerializeField] float bossMoveSpeed = 5f;
    [SerializeField] float rotateSmooth = 0.05f;
    float bossHealth;

    [Header("Action")]
    [SerializeField] float minActionCooldown = 5f;
    [SerializeField] float maxActionCooldown = 10f;
    [SerializeField] float closeRangeThreshold = 10f;
    private float actionCooldownTimer;

    [HideInInspector] public enum State
    {
        cooldown,
        makeDesicion,
        doAction
    }
    public State currentState = State.cooldown;

    // Distance and direction
    float playerDistance;
    Vector3 playerDirection;

    void Start()
    {
        bossHealth = bossMaxHealth;
        actionCooldownTimer = 5f;
    }

    private void FixedUpdate()
    {
        GetDistanceDirection();

        switch (currentState)
        {
            case State.cooldown:
                ChasingPlayer();
                LookingPlayer();
                // Reduce cooldown timer
                actionCooldownTimer -= Time.deltaTime;
                actionCooldownTimer = Mathf.Clamp(actionCooldownTimer, 0, Mathf.Infinity);
                // if cooldown complete, switch state
                if (actionCooldownTimer <= 0)
                {
                    currentState = State.makeDesicion;
                }
                break;
            case State.makeDesicion:
                MakeDecision();
                currentState = State.doAction;
                break;
            case State.doAction:
                break;
        }
    }

    // Control boss chasing behavior
    void ChasingPlayer()
    {
        transform.position += playerDirection * Time.deltaTime * bossMoveSpeed;
    }

    // Control boss looking behavior
    void LookingPlayer()
    {
        transform.forward = Vector3.Lerp(transform.forward, playerDirection, rotateSmooth);
    }

    // Set distance and direction between player
    void GetDistanceDirection()
    {
        Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        Vector3 selfXZ = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        playerDistance = Vector3.Distance(playerXZ, selfXZ);
        playerDirection = (playerXZ - selfXZ).normalized;
    }

    // Simple decision by distance
    void MakeDecision()
    {
        if (playerDistance <= closeRangeThreshold)
        {
            CloseRangeAttack();
        }
        if (playerDistance > closeRangeThreshold)
        {
            LongRangeAttack();
        }
    }

    // Pick a random cooldown in range, reset the cooldown timer, switch state
    // *Attach to the last frame event of attack animation*
    void ResetActionCooldown()
    {
        float randomCooldown = Random.Range(minActionCooldown, maxActionCooldown);
        actionCooldownTimer = randomCooldown;
        currentState = State.cooldown;
    }

    // *Attach to the frame event of attack start*
    public void AttackStart(float damage)
    {
        
    }

    // *Attach to the frame event of attack end*
    public void AttackEnd()
    {

    }

    void CloseRangeAttack()
    {
        // animator.SetTrigger("CloseRangeAttack");
        Debug.Log("Close range attack");
        Invoke("ResetActionCooldown", 5f);
    }

    void LongRangeAttack()
    {
        // animator.SetTrigger("LongRangeAttack");
        Debug.Log("Long range attack");
        Invoke("ResetActionCooldown", 5f);
    }
}
