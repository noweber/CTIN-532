using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneCharacter : MonoBehaviour
{
    public Animator animator;
    public Vector3[] wayPoints;

    private int state;
    private Vector3 position_old;
    private SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        position_old= transform.position;
        animator= GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2f);
        for (int i = 0; i < wayPoints.Length; i++)
        {
            sequence.Append(transform.DOMove(wayPoints[i], 10));
            sequence.AppendInterval(1);
        }
        sequence.SetLoops(-1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float d = (position_old - transform.position).magnitude;
        if (Mathf.Approximately(d,0))
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
            if(position_old.z > transform.position.z)
            {
                renderer.flipX = true;
            }
            else
            {
                renderer.flipX = false;
            }
        }
        position_old = transform.position;
    }
}
