using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Slash : MonoBehaviour
{

    [SerializeField]
    int damage = 3;

    [SerializeField]
    float comboMultiplyer = 1.1f;

    [SerializeField]
    float knockBackDist;

    [SerializeField]
    float HitboxDuration = 0.2f;

    bool hit = false;

    private void Awake()
    {
        OnEnable();
    }
    private void OnEnable()
    {
        
    }

    private void Update()
    {
        HitboxDuration -= Time.deltaTime;
        if (HitboxDuration <= 0)
        {
            Destroy(gameObject);
            Player.combo = hit ? Player.combo: 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            hit = true;
            Vector3 knockBack = (transform.position - other.transform.position).normalized * knockBackDist;
            other.GetComponent<Enemy>().Hit(knockBack, damage * Mathf.Pow(comboMultiplyer, Player.combo));
            Player.combo += 1;
        }
    }
}