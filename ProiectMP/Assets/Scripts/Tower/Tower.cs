using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    [SerializeField]
    private float timeBetweenAttacks;
    [SerializeField]
    private float attackRadius;
    [SerializeField]
    private Projectile projectile;

    private Enemy targetEnemy = null;
    private float attackCounter;
    private bool isAttacking = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        attackCounter -= Time.deltaTime;
        Enemy e = null;
        if (targetEnemy == null || targetEnemy.IsDead)
        {
            e = GetNearestEnemyInRange();
            if (e != null && Mathf.Abs(Vector2.Distance(e.transform.position, transform.position)) <= attackRadius)
            {
                targetEnemy = e;
            }
        }
        else
        {
            if (attackCounter <= 0 )
            {
                isAttacking = true;
                attackCounter = timeBetweenAttacks;
            }
            else
            {
                isAttacking = false;
            }
            if (Mathf.Abs(Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition)) > attackRadius)
            {
                targetEnemy = null;
            }
        }
	}

    void FixedUpdate()
    {
        if (isAttacking == true)
        {
            Attack();
        }
        
    }

    public void Attack()
    {
        Projectile newProjectile = Instantiate(projectile) as Projectile;
        TowerManager.Instance.addProjectile(newProjectile);
        newProjectile.transform.localPosition = transform.localPosition;
        if (newProjectile.ProjectileType == proType.arrow)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Arrow);
        }
        else if (newProjectile.ProjectileType == proType.rock)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Rock);
        }
        else if (newProjectile.ProjectileType == proType.fireball)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Fireball);
        }
        isAttacking = false;
        if (targetEnemy == null)
        {
            Destroy(newProjectile);
        }
        else
        {
            StartCoroutine(MoveProjectile(newProjectile));
        }
    }

    IEnumerator MoveProjectile(Projectile projectile)
    {
        while(getTargetDistance(targetEnemy) > 0.20f && projectile != null && targetEnemy != null &&
            targetEnemy.IsDead == false)
        {
            var dir = targetEnemy.transform.localPosition - transform.localPosition;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition,
                                                    targetEnemy.transform.localPosition, 5f * Time.deltaTime);
            yield return null;
        }
        if (projectile != null || targetEnemy == null)
        {
            Destroy(projectile);
        }
    }

    private float getTargetDistance(Enemy currentEnemy)
    {
        if (currentEnemy == null)
        {
            currentEnemy = GetNearestEnemyInRange();
            if (currentEnemy == null)
            {
                return 0;
            }
        }
        return Mathf.Abs(Vector2.Distance(transform.localPosition, currentEnemy.transform.localPosition));
    }

    private List<Enemy> GetEnemiesInRange()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();
        foreach(Enemy e in GameManager.Instance.enemyList)
        {
            if (Mathf.Abs(Vector2.Distance(e.transform.localPosition, transform.localPosition)) <= attackRadius) { }
            {
                enemiesInRange.Add(e);
            }
        }

        return enemiesInRange;
    }

    private Enemy GetNearestEnemyInRange()
    {
        Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity;
        float currentDistance = 0.0f;

        foreach(Enemy e in GetEnemiesInRange())
        {
            currentDistance = Vector2.Distance(transform.localPosition, e.transform.localPosition);
            if (currentDistance < smallestDistance)
            {
                nearestEnemy = e;
                smallestDistance = currentDistance;
            }
        }

        return nearestEnemy;
    }
}
