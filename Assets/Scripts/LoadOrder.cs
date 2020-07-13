using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOrder : MonoBehaviour
{
    [SerializeField]
    Planet planet = null;

    GeneticNoise PlanetGN;

    private void Awake()
    {
        if(!planet) planet = GameObject.Find("Planet").GetComponent<Planet>();
    }

    void Start()
    {
        if (!StaticSettings.useStaticSettings) return;

        // Démarré l'algo gén de la planete
        PlanetGN = planet.GetComponent<GeneticNoise>();
        PlanetGN.OnEndAlgoGen += EndGeneratePlanet;
        PlanetGN.enabled = true;
    }

    void EndGeneratePlanet()
    {
        PlanetGN.OnEndAlgoGen -= EndGeneratePlanet;
        print("fin planet");
        // Démarré l'algo gén de la végétation
        planet.GetComponent<SpawnerGenerator>().enabled = true;
        planet.GetComponent<GeneticTree>().enabled = true;

        // Démarré le spawn des créatures

    }
}