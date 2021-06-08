using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnSystem : MonoBehaviour
{
    public float TurnSpeed = 10f;
    public bool IsLockOn;
    public List<GameObject> TargetList;
    public GameObject _SelectLockOnTarget;
    public Transform Player;

    [SerializeField]
    int targetIndex;

    PlayerManager PlayerManager;
    private void OnDisable()
    {
        IsLockOn = false;

        if (_SelectLockOnTarget != null)
        {
            _SelectLockOnTarget.GetComponent<EnemyLockOn>().UnLockTarget();
        }
        _SelectLockOnTarget = null;
    }

    private void Awake()
    {
        PlayerManager = GetComponentInParent<PlayerManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !PlayerManager.IsChangeCam && !PlayerManager.IsPause && TargetList.Count != 0)
        {
            IsLockOn = !IsLockOn;
        }

        if (IsLockOn)
        {
            if (_SelectLockOnTarget != null)
            {
                Quaternion lookOnLook = Quaternion.LookRotation(new Vector3(_SelectLockOnTarget.transform.position.x, Player.position.y, _SelectLockOnTarget.transform.position.z) - Player.position);
                Player.rotation = Quaternion.Slerp(Player.rotation, lookOnLook, TurnSpeed * Time.deltaTime);
                PlayerManager.ThirdCam.transform.LookAt(_SelectLockOnTarget.transform.position);

                if (!_SelectLockOnTarget.activeInHierarchy)
                {
                    IsLockOn = false;
                }
            }
            else
            {
                _SelectLockOnTarget = FindNearestTarget();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                _SelectLockOnTarget = FindNearestTarget();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                _SelectLockOnTarget = GetNextTarget();
            }
        }
        else
        {
            if (_SelectLockOnTarget != null)
            {
                _SelectLockOnTarget.GetComponent<EnemyLockOn>().UnLockTarget();
            }
            _SelectLockOnTarget = null;
        }
    }

    GameObject FindNearestTarget()
    {
        float dis;

        if (_SelectLockOnTarget != null)
        {
            _SelectLockOnTarget.GetComponent<EnemyLockOn>().UnLockTarget();
        }

        dis = Vector3.Distance(Player.position, TargetList[0].transform.position);
        _SelectLockOnTarget = TargetList[0].gameObject;
        targetIndex = 0;

        for (int i = 0; i < TargetList.Count; i++)
        {
            float newDis = Vector3.Distance(Player.position, TargetList[i].transform.position);
            if (dis > newDis)
            {
                dis = newDis;
                targetIndex = i;
                _SelectLockOnTarget = TargetList[i].gameObject;
            }
        }

        _SelectLockOnTarget.GetComponent<EnemyLockOn>().TargetLock();

        return _SelectLockOnTarget;
    }

    GameObject GetNextTarget()
    {
        _SelectLockOnTarget.GetComponent<EnemyLockOn>().UnLockTarget();

        if (targetIndex + 1 > TargetList.Count - 1)
        {
            targetIndex = 0;
            _SelectLockOnTarget = TargetList[targetIndex];
        }
        else
        {
            targetIndex++;
            _SelectLockOnTarget = TargetList[targetIndex];

        }

        _SelectLockOnTarget.GetComponent<EnemyLockOn>().TargetLock();
        return _SelectLockOnTarget;
    }

    public void TargetOutOffRange()
    {
        IsLockOn = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyLockOn>(out var enemy) && !other.CompareTag(Player.gameObject.tag))
        {
            TargetList.Add(enemy.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<EnemyLockOn>(out var enemy) && !other.CompareTag(Player.gameObject.tag))
        {
            if (enemy.gameObject == _SelectLockOnTarget)
            {
                TargetOutOffRange();
            }

            TargetList.Remove(enemy.gameObject);
        }
    }
}
