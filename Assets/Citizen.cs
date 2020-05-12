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
    int _objectives = 1; // lista de objetivos a coger
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

    void GetNextObjetive()
    {
        _target = Stage.Instance.Objectives[Random.Range(0, Stage.Instance.Objectives.Count)].position;
    }

    void Start()
    {
        // Siguiente objetivo
        GetNextObjetive();
        _ac = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _target = Stage.Instance.Objectives[Random.Range(0, Stage.Instance.Objectives.Count)].position;
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
                _objectives -= 1;
                // Si sigue teniendo objetivos
                if (_objectives > 0)
                {
                    GetNextObjetive();
                }
                else
                {
                    // Se va al coche
                    _target = Stage.Instance.Cars[Random.Range(0, Stage.Instance.Cars.Count)].position;
                    _agent.SetDestination(_target);
                }
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
