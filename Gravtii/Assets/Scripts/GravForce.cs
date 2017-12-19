﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GravForce : MonoBehaviour
{
    public GameObject planetObj; // Reference to the 3d model of a planet(star)
    public Slider timeSlider;

    private const float gravConst = 5f;
    private const float velConst = 0.1f;
    private const float timeConst = 10f;
    public static float time = 0;

    public static List<PlanetInfo> planets; // Physics information of all planets
    public static bool canPlay = false;
    public static Vector3[] initPoss; // Initial positions before begining the animations

    private List<int> planetsToJoin; // Planet ids that must be joined

    private void Awake()
    {
        planets = new List<PlanetInfo>();
        planetsToJoin = new List<int>();
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

            if (planets.Count >= 1)
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

            float newTConst = timeSlider.value * timeConst;
            //Vector3 newPos = transform.GetChild(i).position + (planets[i].initVel * Time.deltaTime) + acceleration * Mathf.Pow(Time.deltaTime, 2) / 2;
            Vector3 newPos = planets[i].pos + (planets[i].initVel * velConst * Time.deltaTime * newTConst) + acceleration * Mathf.Pow(Time.deltaTime * newTConst, 2) / 2;

            transform.GetChild(i).position = newPos;
            planets[i].pos = newPos;
        }
    }

    private void ManageCollision()
    {
        int objCount = planets.Count;

        for (int i = 0; i < objCount - 1; i++)
        {
            Vector3 gravForce = Vector3.zero;

            for (int j = objCount - 1; j > i; j--)
            {
                //if (j != i)
                //{
                    Vector3 dir = planets[i].pos - planets[j].pos;
                    float dist = Mathf.Pow(dir.x, 2) + Mathf.Pow(dir.y, 2) + Mathf.Pow(dir.z, 2);
                    // If the objects collided, replace them with an object of their combination 
                    float collisionDist = (transform.GetChild(i).localScale.x + transform.GetChild(j).localScale.x) / 2;
                    //Debug.Log(dist + " < " + collisionDist);
                    if (Mathf.Sqrt(dist) < collisionDist)
                    {
                        planetsToJoin.Add(i);
                        planetsToJoin.Add(j);
                        
                        /*
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
                        */
                    //}
                }
            }
        }

        int length = planetsToJoin.Count;

        if (length >= 1)
        {
            Time.timeScale = 0;
            planetsToJoin.Clear();
            return;
        }

        if (length > 1)
        {
            Debug.Log(length);
            planetsToJoin.Sort();

            float combMass = 0;
            Vector3 combPos = Vector3.zero;
            Vector3 combGravForce = Vector3.zero;
            Vector3 combInitVel = Vector3.zero;

            for (int i = length - 1; i >= 0; i--)
            {
                int id = planetsToJoin[i];

                combMass += planets[id].mass;

                combPos += planets[id].pos;
                combGravForce += planets[id].gravForce;
                combInitVel += planets[id].initVel;
                
                planetsToJoin.Remove(id);
                planets.Remove(planets[id]);
                Destroy(transform.GetChild(id).gameObject);
            }

            combPos /= length;

            planets.Add(new PlanetInfo(combMass, combPos, combGravForce, combInitVel));
            GameObject newObj = Instantiate(planetObj, combPos, Quaternion.identity, transform);

            Material newMat = new Material(Shader.Find("Specular"));
            newMat.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            newObj.GetComponent<Renderer>().material = newMat;
            newObj.GetComponent<TrailRenderer>().material = newMat;

            float radius = CalcRadius(combMass);
            newObj.transform.localScale = new Vector3(radius, radius, radius);
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