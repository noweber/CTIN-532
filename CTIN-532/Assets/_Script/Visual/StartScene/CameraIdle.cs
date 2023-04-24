using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CameraIdle : MonoBehaviour
{
    //public Transform target;

    public Vector3[] wayPoints;
    public Vector3[] Angles;

    private void Start()
    {
        if (wayPoints != null && Angles != null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(2f);
            for (int i = 0; i < wayPoints.Length; i++)
            {
                sequence.Append(transform.DOMove(wayPoints[i], 10).SetEase(Ease.InOutCubic));
                sequence.AppendInterval(5);
            }
            for (int i = 0; i < Angles.Length; i++)
            {
                sequence.Insert(i * 15, transform.DORotate(Angles[i], 10).SetEase(Ease.InOutCubic));
            }
            sequence.SetLoops(-1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
