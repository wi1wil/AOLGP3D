using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cinemachine;

public class ProjectileShooting : MonoBehaviour
{
    public Camera cam;
    private Vector3 destination;
    public GameObject projectilePrefab;
    public GameObject parent;
    public Transform LHFirePoint;
    public Transform RHFirePoint;
    public float projectileSpeed = 20f;
    private float timeToFire; 
    public float fireRate = 4f;
    public float arcRange = 1f;

    private bool leftHand;

    void Update()
    {
        if (cam == null)
        {
            var brain = Camera.main.GetComponent<CinemachineBrain>();
            if (brain != null && brain.OutputCamera != null)
                cam = brain.OutputCamera;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1/fireRate;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            destination = hit.point;
        }
        else
        {
            destination = ray.GetPoint(1000f);
        }

        if (leftHand)
        {
            leftHand = false; 
            InstantiateProjectile(LHFirePoint);
        }
        else
        {   
            leftHand = true;
            InstantiateProjectile(RHFirePoint);
        }
    }

    void InstantiateProjectile(Transform firePoint)
    {
        var projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity) as GameObject;
        projectile.transform.SetParent(parent.transform);
        projectile.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;
        iTween.PunchPosition(projectile, new Vector3(Random.Range(-arcRange, arcRange), Random.Range(-arcRange, arcRange), 0), Random.Range(0.5f, 2f));
    }
}
