using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public static Stage Instance;

    [SerializeField]
    Transform _carsParent = null;
    [SerializeField]
    Transform _objectivesParent = null;

    [HideInInspector]
    public List<Transform> Objectives = new List<Transform>(); // Objetivos: Papel higiénico, comida, etc.

    [HideInInspector]
    public List<Transform> Cars = new List<Transform>(); // Resguardo

    [HideInInspector]
    public int Score = 0;

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

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
            Objectives.Add(_objectivesParent.GetChild(i));
        }
    }
}
