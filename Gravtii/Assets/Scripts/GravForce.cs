using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravForce : MonoBehaviour
{
    public GameObject planetObj;

    private const float velConst = 0.05f;
    private const float gravConst = 1;
    public static float time = 0;

    public static List<PlanetInfo> planets;
    public static bool canPlay = false;

    private void Awake()
    {
        planets = new List<PlanetInfo>();
    }

    private void Update()
    {
        Play();
    }

    private void Play()
    {
        if (canPlay == true)
        {
            time += Time.deltaTime;

            if (planets.Count > 1)
            {
                Act();
                ManageCollision();
            }
        }
    }

    private void Act()
    {
        int objCount = planets.Count;

        for (int i = 0; i < objCount; i++)
        {
            Vector3 gravForce = Vector3.zero;

            for (int j = 0; j < objCount; j++)
            {
                if (j != i)
                {
                    Vector3 dir = planets[i].pos - planets[j].pos;
                    float dist = Mathf.Pow(dir.x, 2) + Mathf.Pow(dir.y, 2) + Mathf.Pow(dir.z, 2); // Power of distance between objects

                    if (dist < 0.01f)
                        dist = 0.01f;

                    float forceMagnitude = gravConst * planets[i].mass * planets[j].mass / dist; // Magnitude of the force
                    Vector3 dirNormalized = dir / Mathf.Sqrt(dist); // Direction

                    // Apply the gravitational force of j-th object to i-th object's total force
                    gravForce -= dirNormalized * forceMagnitude;

                    //ManageCollision();
                }
            }

            planets[i].gravForce = gravForce;

            Vector3 acceleration = planets[i].gravForce / planets[i].mass;

            Vector3 newPos = planets[i].pos + (planets[i].initVel * time) * velConst + acceleration * Mathf.Pow(time, 2) / 2;

            transform.GetChild(i).position = newPos;
            planets[i].pos = newPos;
        }
    }

    private void ManageCollision()
    {
        int objCount = planets.Count;

        for (int i = 0; i < objCount; i++)
        {
            Vector3 gravForce = Vector3.zero;

            for (int j = 0; j < objCount; j++)
            {
                if (j != i)
                {
                    Vector3 dir = planets[i].pos - planets[j].pos;
                    float dist = Mathf.Pow(dir.x, 2) + Mathf.Pow(dir.y, 2) + Mathf.Pow(dir.z, 2);
                    // If the objects collided, replace them with an object of their combination 
                    float collisionDist = (transform.GetChild(i).localScale.x + transform.GetChild(j).localScale.x) / 2;
                    if (Mathf.Sqrt(dist) < collisionDist / 4)
                    {
                        float combMass = planets[i].mass + planets[j].mass;
                        Vector3 combPos = (planets[i].pos + planets[j].pos) / 2;
                        Vector3 combGravForce = planets[i].gravForce + planets[j].gravForce;
                        Vector3 combInitVel = planets[i].initVel + planets[j].initVel;

                        planets.Remove(planets[i]);
                        Destroy(transform.GetChild(i).gameObject);

                        int indToRem = j;
                        if (i < j)
                            indToRem--;

                        planets.Remove(planets[i < j ? j-1 : j]);
                        Destroy(transform.GetChild(j).gameObject);

                        planets.Add(new PlanetInfo(combMass, combPos, combGravForce, combInitVel));
                        GameObject newObj = Instantiate(planetObj, combPos, Quaternion.identity, transform);
                        float radius = CalcRadius(combMass);
                        newObj.transform.localScale = new Vector3(radius, radius, radius);

                        return;
                    }
                }
            }
        }
    }

    // M = V * Rho, is used to calculate the merged planet radius
    public static float CalcRadius(float mass)
    {
        return Mathf.Pow((3 / (4 * Mathf.PI)) * mass, 0.333f) * 5;
    }
}

public class PlanetInfo
{
    public float mass;
    public Vector3 pos;
    public Vector3 gravForce;
    public Vector3 initVel;

    public PlanetInfo(float mass, Vector3 pos, Vector3 gravForce, Vector3 initVel)
    {
        this.mass = mass;
        this.pos = pos;
        this.gravForce = gravForce;
        this.initVel = initVel;
    }
}