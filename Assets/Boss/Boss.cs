using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject player;

    [Header("Basic")]
    [SerializeField] float bossMaxHealth = 5f;
    [SerializeField] float bossMoveSpeed = 5f;
    [SerializeField] float bossHealth;

    [Header("Teleport")]
    [SerializeField] float teleportDistance = 3f; // 闪现的距离
    [SerializeField] float teleportDelay = 2f; // 闪现前的倒计时

    [Header("ChargeAttack")]
    [SerializeField] float chargePrepareTime = 1f;// 穿刺前摇
    [SerializeField] float chargeSpeed = 25f;// 穿刺速度
    [SerializeField] float chargeOffset = 3f;

    [Header("Action")]
    [SerializeField] float minActionCooldown = 5f;
    [SerializeField] float maxActionCooldown = 10f;
    [SerializeField] float closeRangeThreshold = 10f;
    private float actionCooldownTimer;

    [Header("Effects")]
    //[SerializeField] GameObject teleportEffect; // 闪现特效
    //[SerializeField] GameObject chargeEffect;   // 冲刺特效
    [SerializeField] ParticleSystem teleportEffect; 
    [SerializeField] ParticleSystem chargeEffect;  

    [HideInInspector] public enum State
    {
        cooldown,
        makeDesicion,
        doAction
    }
    public State currentState = State.cooldown;

    float playerDistance;
    Vector3 playerDirection;
    private NavMeshAgent agent;

    void Start()
    {
        bossHealth = bossMaxHealth;
        actionCooldownTimer = 5f;

        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        GetDistanceDirection();
        SetAgentMovement();

        switch (currentState)
        {
            case State.cooldown:
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            bossHealth = 50.5f;
    }

    void SetAgentMovement()
    {
        agent.destination = player.transform.position;
        agent.speed = bossMoveSpeed;
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
        //一阶段
        if ((bossHealth / bossMaxHealth) >= 2.0f/3.0f)
        {
            if (playerDistance > closeRangeThreshold)
                StartCoroutine(PrepareTeleport(1));
            else
                CloseRangeAttack();
        }
        //二阶段
        else if ((bossHealth / bossMaxHealth) < 2.0f/3.0f && (bossHealth / bossMaxHealth) >= 1.0f/3.0f)
        {
            if (playerDistance > closeRangeThreshold)
                StartCoroutine(PrepareTeleport(2));
            else
                StartCoroutine(ChargeAttack(3));
        }
    }

    void ResetActionCooldown()
    {
        float randomCooldown = Random.Range(minActionCooldown, maxActionCooldown);
        actionCooldownTimer = randomCooldown;
        currentState = State.cooldown;
    }

    IEnumerator StopMoving(float stopTime)
    {
        float savedSpeed = bossMoveSpeed;
        bossMoveSpeed = 0;
        yield return new WaitForSeconds(stopTime);
        bossMoveSpeed = savedSpeed;
    }

    IEnumerator PrepareTeleport(int attackType)
    {
        Debug.Log($"Teleport in {teleportDelay} seconds.");
        yield return new WaitForSeconds(teleportDelay);
        StartCoroutine(TeleportAttack(attackType));
    }

    // 1: CloseRangeAttack
    // 2: TripleChargeAttack
    //闪现攻击
    IEnumerator TeleportAttack(int attackType)
    {
        Vector3 originalPosition = transform.position; 
        Vector3 teleportPosition = player.transform.position + player.transform.forward * teleportDistance; // 计算闪现位置
        transform.position = teleportPosition; 
        
        // Boss 面朝玩家
        Vector3 playerDirectionXZ = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z); 
        //transform.LookAt(playerDirectionXZ); 
        transform.forward = playerDirectionXZ - transform.position;// Boss 强制面向玩家
        
       // yield return new WaitForSeconds(1.5f); // 停顿（完成转向）
        
        //transform.position = originalPosition; // 返回原始位置（效果不好，会贴脸玩家）

        if (attackType == 1)
        {
         yield return new WaitForSeconds(1f); // 停顿（完成转向）
         CloseRangeAttack();
        }
        if (attackType == 2)
            
            StartCoroutine(ChargeAttack(3));
        // ChasingPlayer(); // 继续追踪玩家
        // ResetActionCooldown();
    }

    //穿刺攻击
    IEnumerator ChargeAttack(int times)
    {
        while (times > 0)
        {
           
            // Boss 面朝玩家
            Vector3 playerDirectionXZ = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z); 
            transform.forward = playerDirectionXZ - transform.position;

            //准备
            Debug.Log("Charging...");
            StartCoroutine(StopMoving(chargePrepareTime));
            yield return new WaitForSeconds(chargePrepareTime);
            
             yield return new WaitForSeconds(1f); // 停顿（完成转向）

            // 播放粒子特效
            chargeEffect.Play(); // 播放粒子特效

            //执行冲刺
     
           
            Vector3 targetPos = player.transform.position + playerDirection * chargeOffset;
            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, chargeSpeed * Time.deltaTime);
               // GameObject effect = Instantiate(chargeEffect, transform.position, Quaternion.identity);
               // Destroy(effect, 1f);
                yield return null;
            }



            times--;
        }

        ResetActionCooldown();
    }

    void CloseRangeAttack()
    {
        Debug.Log("Close range attack");
         // 播放攻击特效
          teleportEffect.Play(); // 播放粒子特效
       // GameObject effect = Instantiate(teleportEffect, transform.position, Quaternion.identity);
        //Destroy(effect, 1f); // 完成后销毁特效
        ResetActionCooldown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hit!");
        }
    }
}
