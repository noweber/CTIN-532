using UnityEngine;
using static MapNodeController;

public class Projectile : HurtBox
{
    [SerializeField]
    private float speed = 4;

    [SerializeField]
    private Vector3 direction;

    public Projectile Initialize(Vector3 targetPosition, Player owner)
    {
        direction = targetPosition - this.transform.position;
        transform.LookAt(targetPosition);
        base.Owner = owner;
        return this;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (direction == Vector3.zero)
        {
            Debug.LogError("Projectile's direction was not set.");
            Destroy(gameObject);
        }

        this.transform.position += speed * direction * Time.fixedDeltaTime;
    }

    protected override void DamageHitBox(HitBox hitBox)
    {
        hitBox.ReceiveDamage(Damage);
    }
}
