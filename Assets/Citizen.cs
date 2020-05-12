using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    public static List<Citizen> Citizens = new List<Citizen>();

    enum State
    {
        Idle,
        Running,
        Dead
    }

    [SerializeField]
    GameObject _zombie = null;

    int health = 2;
    State _state = State.Idle;
    NavMeshAgent _agent;
    Animator _ac;

    Vector3 _target = Vector3.zero;

    public void Hit(int damage)
    {
        if (--health <= 0)
        {
            SetState(State.Dead);
        }
    }

    void OnEnable()
    {
        Citizens.Add(this);
    }

    void OnDisable()
    {
        Citizens.Remove(this);
    }

    void Start()
    {
        _ac = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(_target); // Destino del ciudadano
        SetState(State.Running);
    }

    void Update()
    {
        // Si estoy corriendo hacia un sitio
        if (_state == State.Running)
        {
            // Si el ciudadano ha llegado a su destino
            if ((_target - transform.position).sqrMagnitude < 1f)
            {
                // Lo mandamos a otro sitio
                _agent.SetDestination(new Vector3(10f, 0, 10f));
            }
        }
    }

    void SetState(State state)
    {
        if (_state != state)
        {
            _state = state;
            switch (_state)
            {
                case State.Running:
                    _ac.CrossFade("Run", 0.1f); // En vez de hacer un play directo hace una pequeña transición
                    break;

                case State.Dead:
                    _ac.CrossFade("Die", 0.1f);
                    Instantiate(_zombie, transform.position, transform.rotation);
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
