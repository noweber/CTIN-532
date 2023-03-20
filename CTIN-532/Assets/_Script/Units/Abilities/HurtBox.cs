using System.Reflection;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField]
    private GameObject particlesPrefab;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private bool destroyOnContact = false;

    private void OnTriggerEnter(Collider other)
    {
        HitBox hitBox = other.GetComponent<HitBox>();
        if (hitBox != null)
        {
            DamageHitBox(hitBox);
        }
    }

    protected virtual void DamageHitBox(HitBox hurtBox)
    {
        hurtBox.ReceiveDamage(damage);

        if(particlesPrefab != null)
        {
            Instantiate(particlesPrefab, transform);
        }

        if (destroyOnContact)
        {
            Destroy(gameObject);
        }
    }
}
