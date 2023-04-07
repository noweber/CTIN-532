using UnityEngine;
using static MapNodeController;

public class HitBox : MonoBehaviour
{
    public Player Owner;

    [SerializeField]
    public float MaxHitPoints = 1;

    [SerializeField]
    private float currentHitPoints = 1;

    private Hpbar hitPointsBar;

    private void Awake()
    {
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

        if (hitPointsBar != null)
        {
            hitPointsBar.updateHpBar(MaxHitPoints, currentHitPoints);
        }

        if (currentHitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
