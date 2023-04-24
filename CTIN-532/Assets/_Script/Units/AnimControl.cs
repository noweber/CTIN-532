using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControl : MonoBehaviour
{

    private Animator _animator;
    private SpriteRenderer renderer;
    public UnitController unitController;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (unitController != null)
        {
            if (_animator != null)
            {
                _animator.SetBool("isInCombat", unitController.IsInCombat());
            }
            if (unitController.IsFacingLeft())
            {
                renderer.flipX = false;
            }
            else
            {
                renderer.flipX = true;
            }
        }
    }
}
