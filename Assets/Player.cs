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

    State _state;
    Enemy _target;
    AudioSource _audio;

    void Start()
    {
        _ac = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Si el joystick se está moviento no se actualizan los estados
        // Si dejamos de pulsar el joystick va a pasar de corriendo a idle
        // De idle a buscar al zombi
        // Y de encontrar al zombi a disparar

        // Si el joystick no se está moviendo
        if (_input.Direction.sqrMagnitude > 0f)
        {
            // Se mueve
            var direction = transform.forward;
            transform.position += direction * Time.deltaTime * _speed;
            transform.LookAt(transform.position + (new Vector3(_input.Direction.x, 0f, _input.Direction.y)));
            SetState(State.Running);
        }
        else
        {
            UpdateState();
        }
    }

    // Evento al que nos llama la animación cada vez que llega a ese frame (justo al principio)
    public void Shoot()
    {
        _audio.Play();
        _particles.Play();
        if (_target != null) // Si tiene al zombi a la vista
        {
            // Le hace 1 de daño
            _target.Hit(1);
            // Le apuntamos
            _line.enabled = true;
            var target = _target.transform.position;
            target.y = 1f;
            _line.SetPosition(0, _particles.transform.position);
            _line.SetPosition(1, target);
            StopAllCoroutines();
            StartCoroutine(Recoil());
        }
    }

    // Volver a esconder la línea
    IEnumerator Recoil()
    {
        yield return new WaitForSeconds(0.1f);
        _line.enabled = false;
    }

    // Comprueba si el enemigo está en rango
    bool IsInRange(Enemy enemy)
    {
        var dist = (enemy.transform.position - transform.position).sqrMagnitude;
        return dist < _range * _range;
    }

    // Busca al enemigo mas cercano
    Enemy FindClosesEnemyInRange()
    {
        float closest = int.MaxValue;
        Enemy candidate = null;
        float sqRange = _range * _range;
        foreach (var enemy in Enemy.Enemies)
        {
            var dist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (enemy.IsAlive && dist < closest && dist < sqRange)
            {
                closest = dist;
                candidate = enemy;
            }
        }
        return candidate;
    }

    // Dependiendo de cada estado se va a actualizar
    void UpdateState()
    {
        switch (_state)
        {
            case State.Running:
                SetState(State.Idle); // Si no estamos moviendo el Joystick y está corriendo pasa a Idle
                break;
            case State.Idle:
                // Busca al enemigo más cercano
                Enemy candidate = FindClosesEnemyInRange();
                // Si lo encuentra
                if (candidate != null)
                {
                    // Lo ataca
                    _target = candidate; // Guarda a quien estoy disparando
                    transform.LookAt(_target.transform); // Lo mira
                    SetState(State.Attacking); // Lo ataca
                }
                break;
            case State.Attacking:
                // Si se ha cargado al objetivo
                // O no tiene objetivo
                // O el objetivo se ha pirado de su rango
                if (_target == null || !_target.IsAlive || !IsInRange(_target))
                {
                    // Pasamos a buscar objetivo de nuevo
                    SetState(State.Idle);
                }
                break;
            case State.Dead:
                break;
        }
    }

    // Cambia el estado del poli
    void SetState(State state)
    {
        // Si el estado es diferente al actual
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
                case State.Dead:
                    _ac.CrossFade("Die", 0.1f);
                    break;
            }
        }
    }
}