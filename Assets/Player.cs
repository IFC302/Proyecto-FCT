using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Estados del poli
    enum State
    {
        Idle,
        Running,
        Attacking,
        Dead
    }

    [SerializeField]
    Joystick _input;
    [SerializeField]
    Animator _ac;
    [SerializeField]
    ParticleSystem _particles;
    [SerializeField]
    LineRenderer _line;
    [SerializeField]
    float _speed = 5f;
    [SerializeField]
    float _range = 15f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
