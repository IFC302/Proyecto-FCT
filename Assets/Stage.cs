using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    public static Stage Instance;

    public List<Transform> Objectives = new List<Transform>(); // Objetivos: Papel higiénico, comida, etc.

    public List<Transform> Cars = new List<Transform>(); // Resguardo

    void Awake()
    {
        Instance = this;
    }
}
