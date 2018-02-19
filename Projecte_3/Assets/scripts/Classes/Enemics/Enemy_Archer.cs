﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Archer : Enemy
{
    public GameObject bulletPrefab;
    public float attackDistance;
    public float shootCooldown;

    private bool shoot;
    protected Vector3 bulletDirection;

    //Cosntructor
    public Enemy_Archer()
    {

    }

    //Funcions heredades
    public override void Attack()
    {
        bulletPrefab.GetComponent<BulletBehavior>().direction = bulletDirection;
        Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);
    }

    void Awake()
    {
        enabled = true;
    }

    protected override void Start()
    {
        target = GameObject.Find("Player").transform.GetComponent<PlayerBehavior>();
        shoot = true;
    }

    protected override void Update()
    {
        if (Vector3.Distance(transform.position, target.transform.position) <= attackDistance && shoot)
        {
            bulletDirection = (target.transform.position - this.transform.position).normalized;
            Attack();
            shoot = false;
            StartCoroutine(ShootCooldown(shootCooldown));
        }
    }

    IEnumerator ShootCooldown(float s)
    {
        yield return new WaitForSeconds(s);
        shoot = true;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && collision.gameObject.GetComponent<PlayerBehavior>().killEnemy)
            Die();
        else if (collision.transform.tag == "Player")
            collision.gameObject.GetComponent<PlayerBehavior>().Die();
    }
}
