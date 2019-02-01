using UnityEngine;
using RPG.Weapons;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] float attackRadius = 5f;
    [SerializeField] float shotsPerSecond = 0.5f;

    [SerializeField] Projectile projectile = null;
    [SerializeField] Transform projectileSocket = null;

    private bool isAttacking = false;
    private GameObject target;

    public float AttackRadius {
        get { return attackRadius; }
    }

    public void Attack(GameObject target) {
        if (isAttacking) { return; }

        //TODO implement as a coroutine so target can be passed as a parameter
        this.target = target;
        InvokeRepeating("FireProjectile", 0f, 1f / shotsPerSecond);
        isAttacking = true;
    }

    public void EndAttack() {
        if (!isAttacking) { return; }
        
        CancelInvoke();
        isAttacking = false;
    }

    private void FireProjectile() {
        //TODO re-wire attack targetting
        var offset = new Vector3(0, 1f, 0);
        var targetPosition = target.transform.position + offset;
        Vector3 unitVectorToTarget = (targetPosition - projectileSocket.transform.position).normalized;

        //Fire only when pointed (roughly) towards the player
        if (Vector3.Angle(unitVectorToTarget, transform.forward) < 10.0) {
            Projectile newProjectile = Instantiate(projectile, projectileSocket.transform.position, Quaternion.identity);
            newProjectile.Shooter = gameObject;
            float projectileSpeed = newProjectile.Speed;
            newProjectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;
        }
    }
}