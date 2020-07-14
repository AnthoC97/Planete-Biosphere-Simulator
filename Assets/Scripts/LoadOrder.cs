using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOrder : MonoBehaviour
{
    [SerializeField]
    Planet planet = null;

    [SerializeField]
    GameObject simulation = null;

    [SerializeField]
    Text statusText = null;

    GeneticNoise PlanetGN;

    private void Awake()
    {
        if(!planet) planet = GameObject.Find("Planet").GetComponent<Planet>();
    }

    void Start()
    {
        statusText.text = "Status: ready";
        if (!StaticSettings.useStaticSettings) return;

        // Démarré l'algo gén de la planete
        statusText.text = "Status: generating planet...";
        PlanetGN = planet.GetComponent<GeneticNoise>();
        PlanetGN.OnEndAlgoGen += EndGeneratePlanet;
        PlanetGN.enabled = true;

    }

    void EndGeneratePlanet()
    {
        PlanetGN.OnEndAlgoGen -= EndGeneratePlanet;
        print("fin planet");
        statusText.text = "Status: generating trees...";
        // Démarré l'algo gén de la végétation
        planet.GetComponent<SpawnerGenerator>().enabled = true;
        planet.GetComponent<GeneticTree>().enabled = true;

        // Démarré le spawn des créatures
        simulation.SetActive(true);
    }
}
