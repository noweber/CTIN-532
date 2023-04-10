using TMPro;
using UnityEngine;
using static MapNodeController;

public class HitBox : MonoBehaviour
{
    public Player Owner;

    public bool IsBeingHit;

    [SerializeField]
    public float MaxHitPoints = 1;

    [SerializeField]
    private float currentHitPoints = 1;

    private Hpbar hitPointsBar;

    [SerializeField]
    private GameObject damageTextPrefab;

    [SerializeField]
    private float maxSecondsToPausePerHit = 1.0f;

    private float secondsSinceLastHit;

    private void Awake()
    {
        IsBeingHit = false;
        hitPointsBar = GetComponentInChildren<Hpbar>();
    }

    public void SetHitPoints(float value)
    {
        MaxHitPoints = value;
        currentHitPoints = MaxHitPoints;
    }

    public void ReceiveDamage(float rawDamage)
    {
        currentHitPoints -= rawDamage;
        ShowDamageText(rawDamage);

        if (hitPointsBar != null)
        {
            hitPointsBar.updateHpBar(MaxHitPoints, currentHitPoints);
        }

        if (currentHitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (IsBeingHit)
        {
            secondsSinceLastHit += Time.fixedDeltaTime;
            if (secondsSinceLastHit > maxSecondsToPausePerHit)
            {
                IsBeingHit = false;
            }
        }
    }

    private void ShowDamageText(float damage)
    {
        if (damageTextPrefab == null || damage == 0)
        {
            return;
        }
        var gameObject = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, transform);
        var text = gameObject.GetComponent<TextMeshPro>();
        if (text != null)
        {
            text.text = damage.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DoesColliderHaveHostileHurtbox(other))
        {
            IsBeingHit = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (DoesColliderHaveHostileHurtbox(other))
        {
            IsBeingHit = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (DoesColliderHaveHostileHurtbox(other))
        {
            IsBeingHit = true;
        }
    }

    private bool DoesColliderHaveHostileHurtbox(Collider other)
    {
        var hurtBox = other.gameObject.GetComponent<HurtBox>();
        if (hurtBox != null && hurtBox.Owner != this.Owner)
        {
            secondsSinceLastHit = 0;
            return true;
        }
        return false;
    }
}
