using System.Linq;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    // Reference to the UI fields
    private InputField initVelInput;

    private Slider massSlider;
    private Slider posXSlider;
    private Slider posYSlider;
    private Slider posZSlider;

    private Text massText;
    private Text posXText;
    private Text posYText;
    private Text posZText;

    private Button exitButton;

    private void Awake()
    {
        // Initialize UI components
        initVelInput = transform.GetChild(0).GetComponent<InputField>();

        massSlider = transform.GetChild(1).GetComponent<Slider>();
        posXSlider = transform.GetChild(2).GetComponent<Slider>();
        posXSlider.value = UIController.posX;
        posYSlider = transform.GetChild(3).GetComponent<Slider>();
        posZSlider = transform.GetChild(4).GetComponent<Slider>();

        massText = transform.GetChild(5).GetComponent<Text>();
        massText.text = massSlider.value.ToString();
        posXText = transform.GetChild(6).GetComponent<Text>();
        posXText.text = posXSlider.value.ToString();
        posYText = transform.GetChild(7).GetComponent<Text>();
        posYText.text = posYSlider.value.ToString();
        posZText = transform.GetChild(8).GetComponent<Text>();
        posZText.text = posZSlider.value.ToString();

        exitButton = transform.GetChild(9).GetComponent<Button>();
    }

    private void Start()
    {
        // Reference to the functions that will be called when a UI value changes
        initVelInput.onEndEdit.AddListener(delegate { UpdateInitVel(); });

        massSlider.onValueChanged.AddListener(delegate { UpdateMass(); });
        posXSlider.onValueChanged.AddListener(delegate { UpdatePosX(); });
        posYSlider.onValueChanged.AddListener(delegate { UpdatePosY(); });
        posZSlider.onValueChanged.AddListener(delegate { UpdatePosZ(); });

        exitButton.onClick.AddListener(delegate { DeleteItem(); });
    }

    // Update initial velocity of the planet (called when UI changes)
    private void UpdateInitVel()
    {
        float[] vels = initVelInput.text.Split(' ').Select(item => float.Parse(item, CultureInfo.InvariantCulture)).ToArray();

        if (vels.Length >= 3)
        {
            int id = transform.GetSiblingIndex();

            GravForce.planets[id].initVel = new Vector3(vels[0], vels[1], vels[2]);
        }
    }

    private void UpdateMass()
    {
        massText.text = massSlider.value.ToString();

        int id = transform.GetSiblingIndex();

        GravForce.planets[id].mass = massSlider.value;

        Transform planets = GameObject.Find("Planets").transform;
        float radius = GravForce.CalcRadius(massSlider.value);
        planets.GetChild(id).localScale = new Vector3(radius, radius, radius);
    }

    private void UpdatePosX()
    {
        posXText.text = posXSlider.value.ToString();
        
        int id = transform.GetSiblingIndex();

        GravForce.planets[id].pos.x = posXSlider.value;

        Transform planets = GameObject.Find("Planets").transform;
        planets.GetChild(id).position = new Vector3(posXSlider.value, planets.GetChild(id).position.y, planets.GetChild(id).position.z);
    }

    private void UpdatePosY()
    {
        posYText.text = posYSlider.value.ToString();

        int id = transform.GetSiblingIndex();

        GravForce.planets[id].pos.y = posYSlider.value;

        Transform planets = GameObject.Find("Planets").transform;
        planets.GetChild(id).position = new Vector3(planets.GetChild(id).position.x, posYSlider.value, planets.GetChild(id).position.z);
    }

    private void UpdatePosZ()
    {
        posZText.text = posZSlider.value.ToString();

        int id = transform.GetSiblingIndex();

        GravForce.planets[id].pos.z = posZSlider.value;

        Transform planets = GameObject.Find("Planets").transform;
        planets.GetChild(id).position = new Vector3(planets.GetChild(id).position.x, planets.GetChild(id).position.y, posZSlider.value);
    }

    // Delete a UI element of a planet, also remove sthe planet
    private void DeleteItem()
    {
        int id = transform.GetSiblingIndex();

        GameObject planets = GameObject.Find("Planets");

        GravForce.planets.Remove(GravForce.planets[id]);

        Destroy(planets.transform.GetChild(id).gameObject);
        Destroy(gameObject);
    }
}
