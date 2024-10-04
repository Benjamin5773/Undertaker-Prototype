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

    [Header("Teleport")]
    [SerializeField] float teleportDistance = 3f; // ���ֵ����ǰ���ľ���
    [SerializeField] float teleportDelay = 2f; // ����ǰ�ĵ���ʱ

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
                actionCooldownTimer -= Time.deltaTime;
                actionCooldownTimer = Mathf.Clamp(actionCooldownTimer, 0, Mathf.Infinity);
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

    void ChasingPlayer()
    {
        transform.position += playerDirection * Time.deltaTime * bossMoveSpeed;
    }

    void LookingPlayer()
    {
        transform.forward = Vector3.Lerp(transform.forward, playerDirection, rotateSmooth);
    }

    void GetDistanceDirection()
    {
        Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        Vector3 selfXZ = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        playerDistance = Vector3.Distance(playerXZ, selfXZ);
        playerDirection = (playerXZ - selfXZ).normalized;
    }

    void MakeDecision()
    {
        if (playerDistance <= closeRangeThreshold)
        {
            CloseRangeAttack();
        }
        else
        {
            StartCoroutine(PrepareTeleport());
        }
    }

    IEnumerator PrepareTeleport()
    {
        Debug.Log($"Teleport in {teleportDelay} seconds."); // ��ӡ����ʱ
        yield return new WaitForSeconds(teleportDelay);
        StartCoroutine(LongRangeAttack());
    }

    IEnumerator LongRangeAttack()
    {
        Vector3 originalPosition = transform.position; // ��¼ԭʼλ��
        Vector3 teleportPosition = player.transform.position + player.transform.forward * teleportDistance; // ��������λ��
        transform.position = teleportPosition; // ���ֵ����ǰ��
        
        // Boss �泯���
        Vector3 playerDirectionXZ = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z); // ����Y��߶Ȳ���
        transform.LookAt(playerDirectionXZ); // Boss ǿ���������

        yield return new WaitForSeconds(1.5f); // ͣ��1.5��
         //transform.position = originalPosition; // ����ԭʼλ��
        Debug.Log("Long range attack");

        ChasingPlayer(); // ����׷�����
        ResetActionCooldown();
    }

    void ResetActionCooldown()
    {
        float randomCooldown = Random.Range(minActionCooldown, maxActionCooldown);
        actionCooldownTimer = randomCooldown;
        currentState = State.cooldown;
    }

    void CloseRangeAttack()
    {
        Debug.Log("Close range attack");
        Invoke("ResetActionCooldown", 5f);
    }
}
