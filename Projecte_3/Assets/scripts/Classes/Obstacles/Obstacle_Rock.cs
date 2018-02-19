﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Rock : Obstacle {

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (collision.collider.GetComponentInParent<PlayerBehavior>().breakRock)
            {
                this.Die();
                PlayerMechanics.Move(collision.collider.GetComponentInParent<PlayerBehavior>());
            }
            else
                collision.collider.GetComponentInParent<PlayerBehavior>().Die();
        }
    }
}
