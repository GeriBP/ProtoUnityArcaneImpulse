using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    public Image hpBar;
    public float maxHp, fireBallDmg;

    private float currHp;
    private Animator anim;

    void Start()
    {
        currHp = maxHp;
        hpBar.fillAmount = currHp / maxHp;
        anim = FindObjectOfType<Animator>();
    }
    
    public void ChangeHp(float d)
    {
        if (d < 0)
        {
            anim.SetTrigger("hit");
        }
        currHp = Mathf.Clamp(currHp + d, 0f, maxHp);
        if (currHp <= 0f)
        {
            FindObjectOfType<ResetScene>().Reset();
        }
        hpBar.fillAmount = currHp / maxHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireBall"))
        {
            ChangeHp(-fireBallDmg);
        }
    }
}
