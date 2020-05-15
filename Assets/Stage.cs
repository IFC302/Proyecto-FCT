using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    public static Stage Instance;

    [SerializeField]
    Transform _carsParent;
    [SerializeField]
    Transform _objectivesParent;

    [HideInInspector]
    public List<Transform> Objectives = new List<Transform>(); // Objetivos: Papel higiénico, comida, etc.

    [HideInInspector]
    public List<Transform> Cars = new List<Transform>(); // Resguardo

    void Awake()
    {
        Instance = this;

        // Recorremos todos los hijos de este objeto y nos los guardamos como coches
        // Esto lo hacemos para no tener que duplicar los coches y añadirlos todo el rato en la escena
        Cars.Clear();
        for (int i = 0; i < _carsParent.childCount; i++)
        {
            Cars.Add(_carsParent.GetChild(i));
        }

        Objectives.Clear();
        for (int i = 0; i < _objectivesParent.childCount; i++)
        {
            Cars.Add(_objectivesParent.GetChild(i));
        }
    }
}
