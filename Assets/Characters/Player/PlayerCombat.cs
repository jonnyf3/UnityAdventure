using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using RPG.Weapons;

namespace RPG.Characters
{
    public class PlayerCombat : MonoBehaviour
    {
        //public void UseWeapon() {
        //    //TODO don't allow damaging until previous attack animation has finished
        //    foreach (var target in GetDamageablesInRange()) {
        //        target.TakeDamage(CurrentWeapon.Damage);
        //    }
        //}

        //private List<Health> GetDamageablesInRange() {
        //    int mask = ~0;
        //    var objectsInRange = Physics.OverlapSphere(transform.position, CurrentWeapon.Range, mask, QueryTriggerInteraction.Ignore);

        //    var damageables = new List<Health>();
        //    foreach (var obj in objectsInRange) {
        //        //Don't deal damage to self
        //        //TODO could this be done by obj == gameObject to be more general?
        //        if (obj.CompareTag("Player")) { continue; }

        //        var d = obj.gameObject.GetComponentInParent<Health>();
        //        if (d != null) { damageables.Add(d); }
        //    }
        //    return damageables;
        //}
        
        //private void OnDrawGizmos() {
        //   Gizmos.color = Color.green;
        //   Gizmos.DrawWireSphere(transform.position, currentWeapon.Range);
        //}
    }
}