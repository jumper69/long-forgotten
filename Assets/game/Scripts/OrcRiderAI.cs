using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class OrcRiderAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 3f;
    public float wanderRadius = 6f;
    public float wanderInterval = 3f;

    private NavMeshAgent agent;
    private Animator animator;
    private float wanderTimer;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        wanderTimer = wanderInterval;

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                player = found.transform;
            }
            else
            {
                Debug.LogWarning("Player not found.");
            }
        }
    }

    void Update()
    {
        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        if (isAttacking)
        {
            agent.ResetPath();
        }
        else if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(Attack());
        }
        else if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                Vector3 randomDirection = Random.insideUnitCircle * wanderRadius;
                Vector3 wanderTarget = transform.position + new Vector3(randomDirection.x, randomDirection.y, 0f);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(wanderTarget, out hit, 1.5f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }

                wanderTimer = 0f;
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        Vector3 scale = transform.localScale;
        if (agent.velocity.x > 0.1f)
            scale.x = Mathf.Abs(scale.x);
        else if (agent.velocity.x < -0.1f)
            scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);
        agent.ResetPath();

        yield return new WaitForSeconds(1.0f);

        if (player != null)
        {
            HealthSystem targetHealth = player.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(45);
            }
        }

        yield return new WaitForSeconds(0.8f);

        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }
}
