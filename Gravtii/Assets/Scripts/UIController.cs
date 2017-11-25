using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject blocker;

    [Space(10)]

    public GameObject itemObj;

    [Space(10)]

    public Transform planetsParent;
    public GameObject planetObj;

    public static float posX = 0;
    private int length = 0;
    private List<float> mass = new List<float>();
    private List<Vector3> poss = new List<Vector3>();
    private List<Vector3> velss = new List<Vector3>();

    public void AddNewItem()
    {
        // Create a new UI item element
        Transform newItem = Instantiate(itemObj, transform).transform;

        // Create a new planet physics object
        Transform newPlanet = Instantiate(planetObj, new Vector3(posX, 0, 0), Quaternion.identity, planetsParent).transform;

        // Add new planet physics info to its list
        GravForce.planets.Add(new PlanetInfo(10, new Vector3(posX, 0, 0), Vector3.zero, Vector3.zero));

        // Change planet spawn location
        posX += 10;
    }

    // Restart the motion
    public void RestartApplication()
    {
        if (GravForce.canPlay == false)
        {
            length = GravForce.planets.Count;
            mass = GravForce.planets.Select(item => item.mass).ToList();
            poss = GravForce.planets.Select(item => item.pos).ToList();
            velss = GravForce.planets.Select(item => item.initVel).ToList();

            blocker.SetActive(true);
            GravForce.canPlay = true;
        }
        else
        {
            GravForce.planets.Clear();
            for (int i = 0; i < length; i++)
            {
                GravForce.planets.Add(new PlanetInfo(mass[i], poss[i], Vector3.zero, velss[i]));
            }

            GravForce.time = 0;
            ResetInfo();

            blocker.SetActive(false);
            GravForce.canPlay = false;
        }
    }

    private void Update()
    {
        
    } 

    // Exit the application
    public void ExitApplication()
    {
        Application.Quit();
    }

    private void ResetInfo()
    {
        for (int i = 0; i < planetsParent.childCount; i++)
        {
            Destroy(planetsParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < GravForce.planets.Count; i++)
        {
            GameObject newPlanet = Instantiate(planetObj, GravForce.planets[i].pos, Quaternion.identity, planetsParent);
            float radius = GravForce.CalcRadius(GravForce.planets[i].mass);
            newPlanet.transform.localScale = new Vector3(radius, radius, radius);
        }
    }
}