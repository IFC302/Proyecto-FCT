using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public static List<Enemy> Enemies = new List<Enemy>();

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
    float _range = 50f; // Rango de visión que tendrá el zombi
    Citizen _target;

    int lives = 5;

    // Para que el jugador no lo considere como un objetivo válido
    public bool IsAlive
    {
        // Solo queremos que esté vivo si las vidas son > 0
        get { return lives > 0; }
    }

    void Start()
    {
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
                if (_target != null)
                {
                    // Si la distancia es la suficiente para pegarle
                    if ((transform.position - _target.transform.position).sqrMagnitude < 2f)
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
        }
    }

    public void Attack()
    {
        // LLamar a la animación
        // Si no hay dos zombis atacando al mismo
        if (_target != null)
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

    // Cuándo los zombis se activan se registran
    void OnEnable()
    {
        Enemies.Add(this);
    }

    // Cuándo los zombis mueren se retiran
    void OnDisable()
    {
        Enemies.Remove(this);
    }

    // Comprueba si el enemigo está en rango
    bool IsInRange(Citizen enemy)
    {
        var dist = (enemy.transform.position - transform.position).sqrMagnitude;
        return dist < _range * _range;
    }

    // Busca al enemigo mas cercano
    Citizen FindClosestCitizenInRange()
    {
        float closest = int.MaxValue;
        Citizen candidate = null;
        float sqRange = _range * _range;
        foreach (var enemy in Citizen.Citizens)
        {
            var dist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (dist < closest && dist < sqRange)
            {
                closest = dist;
                candidate = enemy;
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
                    _ac.CrossFade("Idle", 0.1f);
                    break;
                case State.Attacking:
                    _agent.isStopped = true;
                    _ac.CrossFade("Attack", 0.1f);
                    break;
                case State.Dead: // Cuándo muere
                    // Animación de morir
                    _ac.CrossFade("Die", 0.1f);
                    // Si hacemos un destroy de this va a destruir el componente Enemy, no el objeto
                    Destroy(gameObject, 2f); // Destruimos el objeto en 2 sec
                    break;
            }
        }
    }
}