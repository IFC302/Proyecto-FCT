using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Cuándo le pegan
    public void Hit(int damage)
    {
        if (lives <= 0)
        {
            //SetState(State.Dead);
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

    void SetState(State state)
    {
        if (state != _state)
        {
            _state = state;
            switch (_state)
            {
                case State.Running:
                    _ac.CrossFade("Run", 0.1f);
                    break;
                case State.Idle:
                    _ac.CrossFade("Idle", 0.1f);
                    break;
                case State.Attacking:
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