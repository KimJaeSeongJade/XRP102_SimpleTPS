using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using UnityEngine.AI;
using TMPro;

public class NormalMonster : Monster, IDamagable
{
    private bool _isActivateControl = true;
    private bool _canTracking = true;

    [SerializeField] private int MaxHp;
    private ObservableProperty<int> CurrentHp;
    private ObservableProperty<bool> IsMoving = new();
    private ObservableProperty<bool> IsAttacking = new();

    private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _targetTransform;

    // private void Awake() => Init();
    // private void Update() => HandleControl();

    // private void Init()
    // {
    //     _navMeshAgent = GetComponent<NavMeshAgent>();
    //     _navMeshAgent.isStopped = true;
    // }

    // private void HandleControl()
    // {
    //     if (!_isActivateControl) return;

    //     HandleMove();
    // }

    // private void HandleMove()
    // {
    //     if (_targetTransform == null) return;

    //     if (_canTracking)
    //     {
    //         _navMeshAgent.SetDestination(_targetTransform.position);
    //     }

    //     _navMeshAgent.isStopped = !_canTracking;
    //     IsMoving.Value = _canTracking;
    // }


    public void TakeDamage(int value)
    {
        // 데미지 판정 구현
        // 체력 깎고
        // 체력이 0 이하가 되면 Dead 처리
        Debug.Log($"{gameObject.name} : {value} 데미지 입음");
    }
}
