using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Linq; 

public class Enemy : MonoBehaviour
{
    public int health = 100;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject weaponFlash;
    public float bloom;
    public float fireRate;
    private float lastShotTime = 0f;


    public Material hitMat;

    public AudioClip shootingSound;

    private Rigidbody rb;
    private Renderer rend;
    private Material originalMat;

    // AI settings
    public float currentPointIndex = 0;
    public Vector3 currentTarget;
    public float positionThreshold;
    public float idleTime = 5f;
    public float attackDistance = 5f;
    public float maxVisionDistance = 20f;
    public float minChasingHealth = 30f;

    public Transform[] patrolPoints;
    private float idleTimeCounter;
    private Transform playerTransform;
    private bool canSeePlayer;
    private Vector3 lastKnownPlayerPosition; 

    private NavMeshAgent agent;

    public enum State { Idle, Patrolling, Chasing, Attacking }
    public State currentState = State.Idle; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalMat = rend.material;
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        GameObject patrolPointParent = GameObject.FindWithTag("PatrolPoints");
        patrolPoints = patrolPointParent.GetComponentsInChildren<Transform>()
            .Where(t => t != patrolPointParent.transform).ToArray();

        idleTimeCounter = idleTime; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Enemy hit by bullet! Health: " + health); // 👈 add this
            health -= 20;
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                Debug.Log("Enemy dying!"); // 👈 add this
                Die();
            }
            else
                StartCoroutine(Blink());
        }
    }

    void Die()
    {
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        
        yield return StartCoroutine(CheckWinCondition());
        Destroy(gameObject);
    }

    IEnumerator CheckWinCondition()
    {
        yield return null;

        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        int aliveCount = 0;

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy != this && enemy.enabled) 
                aliveCount++;
        }

        Debug.Log("Alive enemies: " + aliveCount);

        if (aliveCount == 0)
        {
            Debug.Log("Player won!");
            UIManager.Instance.EnableWinUI();
        }
    }



    IEnumerator Blink()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = originalMat;
    }

    private void Update()
    {
        LookForPlayer();

        switch (currentState) 
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrolling:
                Patrolling();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.Chasing:
                Chasing();
                break;
        }

        rb.linearVelocity = Vector3.zero;
        LookAtPlayer();
        SetLastKnownPlayerPosition();
    }

    private void LookForPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer,
            out RaycastHit hit, maxVisionDistance))
        {
            canSeePlayer = hit.transform == playerTransform;

            if (canSeePlayer && currentState != State.Attacking) 
                currentState = State.Chasing; 
        }
    }

    private void Idle()
    {
        agent.ResetPath();
        idleTimeCounter -= Time.deltaTime; 
        if (idleTimeCounter <= 0)
        {
            currentState = State.Patrolling;
            idleTimeCounter = idleTime;
        }
    }

    private void Patrolling()
    {
        if (Vector3.Distance(currentTarget, transform.position) < positionThreshold)
        {
            float chance = Random.Range(0, 100);
            if (chance < 10)
            {
                currentState = State.Idle; 
                return;
            }
            currentPointIndex++;
            currentTarget = patrolPoints[(int)currentPointIndex % patrolPoints.Length].position;
        }
        else
        {
            agent.SetDestination(currentTarget);
        }
    }

    private void Attacking()
    {
        idleTimeCounter = idleTime;
        agent.ResetPath();
        Shoot();

        if (Vector3.Distance(transform.position, playerTransform.position)
            > attackDistance || !canSeePlayer)
        {
            if (health > minChasingHealth)
                currentState = State.Patrolling; 
            else
                currentState = State.Chasing; 
        }
    }

    private void Chasing()
    {
        idleTimeCounter = idleTime; 
        agent.SetDestination(lastKnownPlayerPosition); 

        if (health < minChasingHealth)
            currentState = State.Patrolling; 
        else if (Vector3.Distance(transform.position, playerTransform.position)
            < attackDistance && canSeePlayer)
            currentState = State.Attacking; 
        else if (Vector3.Distance(transform.position, playerTransform.position)
            > maxVisionDistance)
            currentState = State.Patrolling; 
        else if (Vector3.Distance(transform.position, playerTransform.position)
            > positionThreshold && !canSeePlayer)
            currentState = State.Patrolling; 
    }

    private void LookAtPlayer()
    {
        if (canSeePlayer)
        {
            transform.LookAt(new Vector3(
                playerTransform.position.x,
                transform.position.y,
                playerTransform.position.z));
        }
    }

    private void SetLastKnownPlayerPosition()
    {
        if (canSeePlayer)
            lastKnownPlayerPosition = playerTransform.position; 
    }

    private void Shoot()
    {
        if (Time.time - lastShotTime < fireRate) return; 

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.Normalize();

        Quaternion bulletRotation = Quaternion.LookRotation(directionToPlayer);

        float maxInaccuracy = 10f;
        float currentInaccuracy = bloom * maxInaccuracy;
        float randomYaw = Random.Range(-currentInaccuracy, currentInaccuracy);
        float randomPitch = Random.Range(-currentInaccuracy, currentInaccuracy);

        bulletRotation *= Quaternion.Euler(randomPitch, randomYaw, 0f);

        AudioManager.Instance.PlaySFX(shootingSound, 0.5f);

        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
        Instantiate(weaponFlash, bulletSpawnPoint.position, bulletRotation); 
        lastShotTime = Time.time;
    }
}