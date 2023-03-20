using System.Reflection;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    private int hitPoints = 1;

    public void ReceiveDamage(int rawDamage)
    {
        Debug.Log(MethodBase.GetCurrentMethod());
        hitPoints -= rawDamage;

        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
