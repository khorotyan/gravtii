using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Toggle pauseToggle;
    public Toggle rungeToggle;
    public GameObject blocker; // Reference to the UI blocker when animation begins

    [Space(10)]

    public GameObject itemObj; // Reference to a UI element for one planet(star)

    [Space(10)]

    public Transform planetsParent; // Reference to the container of the planets
    public static GameObject plp;
    public GameObject planetObj; // Reference to the 3d model of the planet

    public static float posX = 0;
    public static float posZ = 0;
    private int length = 0;
    private List<float> mass = new List<float>();
    private List<Vector3> poss = new List<Vector3>();
    private List<Vector3> velss = new List<Vector3>();

    private void Awake()
    {
        //plp = planetsParent.gameObject;
    }

    private void Start()
    {
        pauseToggle.onValueChanged.AddListener(delegate { PauseResume(); });
        rungeToggle.onValueChanged.AddListener(delegate { OnCalcModeChange(); });
    }

    public void PauseResume()
    {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
    }

    public void OnCalcModeChange()
    {
        GravForce.isRunge = !GravForce.isRunge;
    }

    public void AddNewItem()
    {
        // Create a new UI item element
        Transform newItem = Instantiate(itemObj, transform).transform;

        // Create a new planet physics object
        Transform newPlanet = Instantiate(planetObj, new Vector3(posX, 0, posZ), Quaternion.identity, planetsParent).transform;

        Material newMat = new Material(Shader.Find("Specular"));
        newMat.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        newPlanet.GetComponent<Renderer>().material = newMat;
        newPlanet.GetComponent<TrailRenderer>().material = newMat;

        int transPos = transform.childCount - 1;
        transform.GetChild(transPos).GetChild(10).GetComponent<Text>().color = newMat.color;
        transform.GetChild(transPos).GetChild(11).GetComponent<Text>().color = newMat.color;
        transform.GetChild(transPos).GetChild(12).GetComponent<Text>().color = newMat.color;

        // Add new planet physics info to its list
        GravForce.planets.Add(new PlanetInfo(10, new Vector3(posX, 0, posZ), Vector3.zero, Vector3.zero));

        // Change planet spawn location
        posX = Random.Range(-160f, 75f);
        posZ = Random.Range(-90f, 90f);
    }

    // Restart the motion
    public void RestartApplication()
    {
        // Start the animation
        if (GravForce.canPlay == false)
        {
            Time.timeScale = 1;

            length = GravForce.planets.Count;
            mass = GravForce.planets.Select(item => item.mass).ToList();
            poss = GravForce.planets.Select(item => item.pos).ToList();
            velss = GravForce.planets.Select(item => item.initVel).ToList();

            GravForce.initPoss = new Vector3[length];
            GravForce.initPoss = poss.ToArray();

            blocker.SetActive(true);
            GravForce.canPlay = true;

            for (int i = 0; i < planetsParent.transform.childCount; i++)
            {
                planetsParent.transform.GetChild(i).GetComponent<TrailRenderer>().enabled = true;
            }
        }
        else // Stop the animation
        {
            pauseToggle.interactable = true;
            GravForce.planets.Clear();
            for (int i = 0; i < length; i++)
            {
                GravForce.planets.Add(new PlanetInfo(mass[i], poss[i], Vector3.zero, velss[i]));
            }

            GravForce.time = 0;
            ResetInfo();

            blocker.SetActive(false);
            //GravForce.animStopped = false;
            GravForce.canPlay = false;

            for (int i = 0; i < planetsParent.transform.childCount; i++)
            {
                planetsParent.transform.GetChild(i).GetComponent<TrailRenderer>().enabled = false;
            }
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

            transform.GetChild(i).GetChild(10).GetComponent<Text>().color = newMat.color;
            transform.GetChild(i).GetChild(11).GetComponent<Text>().color = newMat.color;
            transform.GetChild(i).GetChild(12).GetComponent<Text>().color = newMat.color;

            float radius = GravForce.CalcRadius(GravForce.planets[i].mass);
            newPlanet.transform.localScale = new Vector3(radius, radius, radius);
        }
    }
}