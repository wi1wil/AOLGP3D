using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class ZombieScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public float moveSpeed = 2f;
    public float detectionRadius = 100f;
    public int hp;
    public int attack = 10;

    private Animator animator;
    private float wanderTimer = 3f;
    private float timer;
    public bool takingDamage = false;
    public TMP_Text hpText;
    public string canvasName = "Canvas";
    public string hpTextParentName = "ZombieHP";

    private Camera mainCam;
    private GameObject player;
    private float attackCooldown = 1.0f;
    private float lastAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.speed = moveSpeed;
        timer = wanderTimer;
        mainCam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");

        GameObject canvasObj = GameObject.Find(canvasName);
        Transform parent = canvasObj != null ? canvasObj.transform : null;
        if (canvasObj != null)
        {
            Transform foundParent = canvasObj.transform.Find(hpTextParentName);
            if (foundParent != null)
                parent = foundParent;
        }

        if (hpText != null && parent != null)
        {
            TMP_Text hpTextInstance = Instantiate(hpText, parent);
            hpText = hpTextInstance;
        }

        UpdateHpText();
    }

    void Update()
    {
        if (player != null && !takingDamage)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= detectionRadius)
            {
                agent.SetDestination(player.transform.position);
                animator.SetBool("isWalking", true);
                timer = 0f;
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= wanderTimer || agent.remainingDistance < 0.5f)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, detectionRadius, -1);
                    agent.SetDestination(newPos);
                    timer = 0f;
                }
                animator.SetBool("isWalking", true);
            }

            if (distance <= 0.02f || IsTouchingPlayer())
            {
                TryAttackPlayer();
            }
        }
        UpdateHpText();
    }

    void LateUpdate()
    {
        if (hpText != null && mainCam != null)
        {
            hpText.transform.position = mainCam.WorldToScreenPoint(transform.position + Vector3.up * 2f);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection.y = 0;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void tookDamaged()
    {
        StartCoroutine(DamageAndResume());
        UpdateHpText();
        if (hp <= 0 && hpText != null)
        {
            Destroy(hpText.gameObject);
            hpText = null;
        }
    }

    private void UpdateHpText()
    {
        if (hpText != null)
            hpText.text = "HP: " + hp.ToString();
    }

    private IEnumerator DamageAndResume()
    {
        takingDamage = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isDamaged", true);
        agent.isStopped = true;
        yield return new WaitForSeconds(2f);
        takingDamage = false;
        animator.SetBool("isDamaged", false);
        animator.SetBool("isWalking", true);
        agent.isStopped = false;
    }

    private bool IsTouchingPlayer()
    {
        if (player == null) return false;
        Collider playerCol = player.GetComponent<Collider>();
        Collider zombieCol = GetComponent<Collider>();
        if (playerCol != null && zombieCol != null)
            return zombieCol.bounds.Intersects(playerCol.bounds);
        return false;
    }

    private void TryAttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
            var playerScript = player.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attack);
            }
            Debug.Log("Zombie attacked the player!");
        }
    }
}
