using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject blocker; // Reference to the UI blocker when animation begins

    [Space(10)]

    public GameObject itemObj; // Reference to a UI element for one planet(star)

    [Space(10)]

    public Transform planetsParent; // Reference to the container of the planets
    public GameObject planetObj; // Reference to the 3d model of the planet

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

        Material newMat = new Material(Shader.Find("Specular"));
        newMat.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        newPlanet.GetComponent<Renderer>().material = newMat;
        newPlanet.GetComponent<TrailRenderer>().material = newMat;

        // Add new planet physics info to its list
        GravForce.planets.Add(new PlanetInfo(10, new Vector3(posX, 0, 0), Vector3.zero, Vector3.zero));

        // Change planet spawn location
        posX += 10;
    }

    // Restart the motion
    public void RestartApplication()
    {
        // Start the animation
        if (GravForce.canPlay == false)
        {
            Time.timeScale = 1;

            /*
            for (int i = 0; i < planetObj.transform.childCount; i++)
            {
                planetObj.transform.GetChild(i).GetComponent<TrailRenderer>().enabled = true;
            }
            */

            length = GravForce.planets.Count;
            mass = GravForce.planets.Select(item => item.mass).ToList();
            poss = GravForce.planets.Select(item => item.pos).ToList();
            velss = GravForce.planets.Select(item => item.initVel).ToList();

            GravForce.initPoss = new Vector3[length];
            GravForce.initPoss = poss.ToArray();

            blocker.SetActive(true);
            GravForce.canPlay = true;
        }
        else // Stop the animation
        {
            /*
            for (int i = 0; i < planetObj.transform.childCount; i++)
            {
                planetObj.transform.GetChild(i).GetComponent<TrailRenderer>().enabled = false;
            }
            */

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

            Material newMat = new Material(Shader.Find("Specular"));
            newMat.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            newPlanet.GetComponent<Renderer>().material = newMat;
            newPlanet.GetComponent<TrailRenderer>().material = newMat;

            float radius = GravForce.CalcRadius(GravForce.planets[i].mass);
            newPlanet.transform.localScale = new Vector3(radius, radius, radius);
        }
    }
}