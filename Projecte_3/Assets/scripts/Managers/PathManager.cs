﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour {

#region Variables

    [Range(0.25f, 0.75f)]
    public float sphereSize;
    [Space][Header("Mechanics constrains")]
    [Space][Range(0.5f, 5.0f)]
    public float constrainDistance;
    [Range(15, 180)]
    public int constrainAngle;
    [HideInInspector]
    public bool constrain;
    private int constrainIndex;
    private bool onIndexCorroutine;

    [Space][Header("Path following")]
    [Space][Range(0.5f, 5.0f)]
    public float arrivalDistance;
    public Vector3[] path;
    private int index;

#endregion

    private void Awake()
    {
        onIndexCorroutine = false;
        constrain = false;
        index = 0;
        constrainIndex = 0;
    }

    //Function to draw the path in the scene when the GameObject is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 v in path)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(v, sphereSize);
            Gizmos.DrawWireSphere(v, arrivalDistance);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(v, constrainDistance);
        }
        Gizmos.color = Color.red;
        for (int i = 1; i < path.Length; i++)
            Gizmos.DrawLine(path[i - 1], path[i]);
    }

#region Functions

    /// <summary>
    /// Actualitza l'index actual del path
    /// </summary>
    /// <param name="player"></param>
    public void ReachedPoint(PlayerBehavior player)
    {
        if(index < path.Length - 1)
        {
            if(Vector3.Distance(player.transform.position, path[index]) <= arrivalDistance)
                index++;
        }
    }

    /// <summary>
    /// Actualitza l'index de la limitació de mecàniques al arribar a un gir
    /// </summary>
    /// <param name="player"></param>
    public void ReachedConstrainPointPoint(PlayerBehavior player)
    {
        if(constrainIndex < path.Length - 1)
        {
            if (Vector3.Distance(player.transform.position, path[constrainIndex]) <= constrainDistance)
            {
                if (!onIndexCorroutine)
                {
                    onIndexCorroutine = true;
                    StartCoroutine(NextIndex(player));
                }
            }
        }
    }

    /// <summary>
    /// Limita l'ús de mecàniques a les curves
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool MechanicConstrains(PlayerBehavior player)
    {
        if(index < path.Length - 1)
        {
            if(Vector3.Distance(player.transform.position, path[constrainIndex]) <= constrainDistance)
            {
                if (Vector3.Angle(player.transform.forward, path[constrainIndex + 1] - path[constrainIndex]) >= constrainAngle)
                {
                    if(!constrain)
                        return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Funció per a seguir el path preestablert
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public Vector3 PathFollowing(PlayerBehavior player)
    {
        //direction
        Vector3 desired_velocity = (path[index] - player.transform.position).normalized * player.maxSpeed;

        //steering force
        Vector3 steering = desired_velocity - player.GetComponent<Rigidbody>().velocity;
        steering = Vector3.ClampMagnitude(steering, player.maxForce);
        steering = steering / player.GetComponent<Rigidbody>().mass;

        return Vector3.ClampMagnitude(player.GetComponent<Rigidbody>().velocity + steering, player.maxSpeed);
    }


    /// <summary>
    /// Funció que retorna un valor per a la arribada a l'ultim punt per a reduir la velocitat del player
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public float ArriveCorrection(PlayerBehavior player)
    {
        float speed_factor = 1.0f;
        float dist = Vector3.Distance(player.transform.position, path[index]);
        if (dist <= arrivalDistance)
        {
            speed_factor = dist / arrivalDistance;
        }
        return speed_factor;
    }

    private bool corroutineAuxiliar(PlayerBehavior player)
    {
        return Vector3.Distance(player.transform.position, path[constrainIndex]) > constrainDistance;
    }

#endregion

#region Corroutines
    private IEnumerator NextIndex(PlayerBehavior player)
    {
        yield return new WaitUntil(() => corroutineAuxiliar(player));
        constrainIndex++;
        onIndexCorroutine = false;
    }

    public IEnumerator ResetMechanics(PlayerBehavior player)
    {
        yield return new WaitUntil(() => corroutineAuxiliar(player));
        player.canTap = true;
        player.canDrag = true;

        player.breakRock = false;
        player.killEnemy = false;

        constrain = false;
    }
#endregion
}
