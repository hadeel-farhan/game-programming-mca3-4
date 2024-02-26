using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }

    public FSMStates currentState;
    
    public float attackDistance = 5;
    public float chaseDistance = 10;
    public float enemySpeed =5;
    public float speed = 3;
    public GameObject player;
    public GameObject handTip;
    public GameObject[] spellProjectiles;
    public float shootRate = 5;
    public GameObject deadVFX;
    public float projectileSpeed = 100;
    public int damageAmount = 20;


    GameObject[] wanderPoints;
    Vector3 nextDestination;
    Animator anim;
    float distanceToPlayer;
    float elapsedTime = 0;


    int currentDestinationIndex = 0;
    Transform deadTransform;
    bool isDead;

    NavMeshAgent agent;
    public Transform enemyEyes;
    public float fieldOfView = 45f;

    // Start is called before the first frame update
    void Start()
    {

        wanderPoints = GameObject.FindGameObjectsWithTag("WanderPoint");
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        handTip = GameObject.FindGameObjectWithTag("HandTip");
        isDead = false;
        agent = GetComponent<NavMeshAgent>();
        Initialize();

    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        switch(currentState)
        {
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Dead:
                UpdateDeadState();
                break;
        }
        elapsedTime += Time.deltaTime;


    
    }

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Projectile")){
            currentState = FSMStates.Dead;
        }
        if (other.CompareTag("Player")) {
            // apply damage
            var playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageAmount);
        }
    }

    void Initialize() {
        currentState = FSMStates.Patrol;
        FindNextPoint(); 

    }

    void UpdatePatrolState() {
        print("Patrolling");

        anim.SetInteger("animState", 1);

        agent.stoppingDistance = 0;

        agent.speed = 3.5f;


        if(Vector3.Distance(transform.position, nextDestination) < 1) {
            FindNextPoint();
        }
        else if(IsPlayerInClearFOV()) {
            currentState = FSMStates.Chase;
        }

        FaceTarget(nextDestination);

        agent.SetDestination(nextDestination);
    }

    void UpdateChaseState() {
        print("Chasing!");
        anim.SetInteger("animState", 2);

        nextDestination = player.transform.position;

        agent.stoppingDistance = attackDistance;

        agent.speed = 5;

        if(distanceToPlayer <= attackDistance){
            currentState = FSMStates.Attack;
        }
        else if(distanceToPlayer > chaseDistance) {
            FindNextPoint();
            currentState = FSMStates.Patrol;
        }

        FaceTarget(nextDestination);
        
        agent.SetDestination(nextDestination);
    }

    void UpdateAttackState() {
        print("attack!");

        nextDestination = player.transform.position;

        if(distanceToPlayer <= attackDistance) {
            currentState = FSMStates.Attack;
        }
        else if (distanceToPlayer > attackDistance && distanceToPlayer <= chaseDistance) {
            currentState = FSMStates.Chase;
        }
        else if (distanceToPlayer > chaseDistance) {
            currentState = FSMStates.Patrol;
        }

        FaceTarget(nextDestination);
        anim.SetInteger("animState", 3);
        Invoke("SpellCasting", 3);
    }

    void UpdateDeadState() {
        anim.SetInteger("animState", 4);
        deadTransform = gameObject.transform;
        isDead = true;

        LevelManager.score += 1;

        Destroy(gameObject);

    }

    void FindNextPoint() {
        nextDestination = wanderPoints[currentDestinationIndex].transform.position;

        currentDestinationIndex = (currentDestinationIndex + 1) % wanderPoints.Length;

        agent.SetDestination(nextDestination);

    }

    void FaceTarget(Vector3 target) {
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10* Time.deltaTime);
    }

    void SpellCasting() {
        if(!isDead){
            if (elapsedTime >= shootRate) {
                GameObject spellProjectile = spellProjectiles[Random.Range(0, spellProjectiles.Length)];
                GameObject projectile = Instantiate(spellProjectile, handTip.transform.position, handTip.transform.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
                projectile.transform.SetParent(GameObject.FindGameObjectWithTag("ProjectileParent").transform);
                elapsedTime = 0.0f;
            }
        }

    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Vector3 frontRayPoint = enemyEyes.position + (enemyEyes.forward * chaseDistance);
        Vector3 leftRayPoint = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * frontRayPoint;
        Vector3 rightRayPoint = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * frontRayPoint;

        Debug.DrawLine(enemyEyes.position, frontRayPoint, Color.cyan);
        Debug.DrawLine(enemyEyes.position, leftRayPoint, Color.yellow);
        Debug.DrawLine(enemyEyes.position, rightRayPoint, Color.yellow);

    }

    bool IsPlayerInClearFOV() {
        RaycastHit hit;

        Vector3 distanceToPlayer = player.transform.position - enemyEyes.position;

        if(Vector3.Angle(distanceToPlayer, enemyEyes.forward) <= fieldOfView) {
            if(Physics.Raycast(enemyEyes.position, distanceToPlayer, out hit, chaseDistance)) {
                if(hit.collider.CompareTag("Player")) {
                    print("Player in sight!");
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;

    }

}
