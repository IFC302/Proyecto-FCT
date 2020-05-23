using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum State
    {
        Idle,
        Running,
        Attacking,
        Dead
    }

    State _state = State.Idle;

    Animator _ac;
    NavMeshAgent _agent;
    Citizen _target;

    int lives = 2;

    // Para que el jugador no lo considere como un objetivo válido
    public bool IsAlive
    {
        // Solo queremos que esté vivo si las vidas son > 0
        get { return lives > 0; }
    }

    void Start()
    {
        Stage.Instance.AddZombie(this);
        _ac = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Si el estado es Idle
        switch (_state)
        {
            case State.Idle:
                // Busca al oponente más cercano
                var citizen = FindClosestCitizenInRange();
                // Si ha encontrado a alguno
                if (citizen != null)
                {
                    // Lo perseguimos
                    _target = citizen;
                    SetState(State.Running);
                }
                break;
            case State.Running:
                // Si está vivo
                var c = FindClosestCitizenInRange();
                _target = c;
                if (_target != null && _target.IsAlive)
                {
                    // Si la distancia es la suficiente para pegarle
                    if ((transform.position - _target.transform.position).sqrMagnitude < 4f)
                    {
                        SetState(State.Attacking);
                    }
                    else
                    {
                        _agent.SetDestination(_target.transform.position);
                    }
                }
                else
                {
                    SetState(State.Idle);
                }
                break;
            case State.Attacking:
                if (_target != null && _target.IsAlive)
                {
                    transform.LookAt(_target.transform);
                }
                break;
        }
    }

    public void Attack()
    {
        // LLamar a la animación
        // Si no hay dos zombis atacando al mismo
        if (_target != null && _target.IsAlive)
        {
            _target.Hit(1);
        }
    }

    public void AttackEnd()
    {
        SetState(State.Running);
    }

    // Cuándo le pegan
    public void Hit(int damage)
    {
        if (--lives <= 0)
        {
            SetState(State.Dead);
        }
    }

    Citizen FindClosestCitizenInRange()
    {
        float closest = int.MaxValue;
        Citizen candidate = null;
        foreach (var citizen in Stage.Instance.Citizens)
        {
            var dist = (citizen.transform.position - transform.position).sqrMagnitude;
            if (citizen.IsAlive && dist < closest)
            {
                closest = dist;
                candidate = citizen;
            }
        }
        return candidate;
    }

    void SetState(State state)
    {
        if (state != _state)
        {
            _state = state;
            switch (_state)
            {
                case State.Running:
                    _agent.isStopped = false;
                    _ac.CrossFade("Run", 0.1f);
                    break;
                case State.Idle:
                    _agent.isStopped = true;
                    _ac.CrossFade("Idle", 0.1f);
                    break;
                case State.Attacking:
                    _agent.isStopped = true;
                    _ac.CrossFade("Attack", 0.1f);
                    break;
                case State.Dead: // Cuándo muere
                    // Animación de morir
                    Stage.Instance.RemoveZombie(this);
                    _agent.isStopped = true; // Desactivamos el Nav Mesh Agent
                    _ac.CrossFade("Die", 0.1f);
                    // Si hacemos un destroy de this va a destruir el componente Enemy, no el objeto
                    Destroy(gameObject, 2f); // Destruimos el objeto en 2 sec
                    break;
            }
        }
    }
}
