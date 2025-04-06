using UnityEngine;

public class GroundImpact : MonoBehaviour
{
    public Player player;
    [Header("Impact Settings")]
    public float impactRadius = 3f;
    public float upwardForce = 10f;
    public float horizontalForce = 5f;
    public float minForce = 3f;
    public LayerMask enemyLayer;

    [Header("Visuals")]
    public ParticleSystem impactEffect;
    public AnimationCurve forceFalloff;

    public void PerformImpact()
    {
        if (impactEffect != null)
        {
            impactEffect.Play();
        }

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, impactRadius, enemyLayer);

        int rand = Random.Range(1, 4);
        for (int i = 0; i < rand; i++)
        {
            var j = SpotManager.Instance.SpawnSpot();
            j.transform.position = transform.position + Vector3.up*2;

            int direction = Random.Range(0, 2) * 2 - 1;

            float randomAngle = Random.Range(30f, 60f);

            Vector2 forceDirection = new Vector2(
                direction * Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                Mathf.Sin(randomAngle * Mathf.Deg2Rad)
            ).normalized;

            float randomForce = Random.Range(10f, 20f);

            j.GetComponent<Rigidbody2D>().AddForce(forceDirection * randomForce, ForceMode2D.Impulse);

        }
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.TryGetComponent<Rigidbody2D>(out var enemyRb))
            {
                if(enemy == null) continue;
                StartCoroutine(enemy.GetComponent<Enemy>().Stunning());
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                float normalizedDistance = Mathf.Clamp01(distance / impactRadius);
                float forceMultiplier = forceFalloff.Evaluate(1 - normalizedDistance);

                Vector2 force = new Vector2(
                    direction.x * horizontalForce * 2 * forceMultiplier,
                    upwardForce * forceMultiplier
                );

                enemyRb.AddForce(Vector2.Max(force, new Vector2(0, minForce)), ForceMode2D.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Рисуем радиус воздействия
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}