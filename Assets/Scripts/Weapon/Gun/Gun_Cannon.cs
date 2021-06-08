using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class Gun_Cannon : GunCore
{
    public LockOnSystem LockOnSystem;
    public GroundMode GroundMode;

    [Space]
    public int resolution = 30;
    public float curveHight = 25;
    public float gravity = -18;
    public float FireRate = 1f;

    [Header("Target Setting")]
    float ManualTargetSpeed = 200f;
    float AutoTargetSpeed = 5f;
    float TargetSpeed;
    float TargetAcculate = 1f;
    public Transform startPoint;
    public Transform target;
    public Transform Player;

    Vector3 defPos;
    bool isDefPos = true;

    [Space]
    public LineRenderer lineRenderer;
    public Animator anima;

    public float t_dis;
    public float s_dis;
    public float d_dis;

    bool isFire;

    [SerializeField]
    Vector3 def_Gravity = new Vector3(0, -10, 0);

    private void OnEnable()
    {
        target.gameObject.SetActive(true);

        if (!IsLocalPlayer)
        {
            return;
        }

        defPos = target.transform.localPosition;
        GroundMode.IsCanHover = false;
    }
    private void OnDisable()
    {
        target.gameObject.SetActive(false);

        if (!IsLocalPlayer)
        {
            return;
        }

        GroundMode.IsCanHover = true;
    }
    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        DrawPath();

        if (Input.GetButtonDown("Fire1") && !playerManager.IsPause && !isFire)
        {
            isFire = true;
            LaunchServerRpc();
            Invoke("ResetFire", FireRate);
        }

        if (!LockOnSystem.IsLockOn)
        {
            if(!isDefPos && target.transform.localPosition != defPos){
                target.transform.localPosition = Vector3.Lerp(target.transform.localPosition, defPos,TargetSpeed * Time.deltaTime);
            }
            else
            {
                isDefPos = true;
            }

            TargetSpeed = ManualTargetSpeed;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                float dis = Vector3.Distance(target.transform.position, Player.transform.position);
                if (Input.GetAxis("Mouse ScrollWheel") > 0f && dis < 100f)
                {
                    target.transform.localPosition += Vector3.forward * TargetSpeed * Time.deltaTime;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f && dis > 40f)
                {
                    target.transform.localPosition += Vector3.back * TargetSpeed * Time.deltaTime;
                }
            }
        }
        else if (LockOnSystem.IsLockOn)
        {
            isDefPos = false;
            TargetSpeed = AutoTargetSpeed;
            t_dis = Vector3.Distance(new Vector3(LockOnSystem._SelectLockOnTarget.transform.position.x, 0f, LockOnSystem._SelectLockOnTarget.transform.position.z),new Vector3(target.transform.position.x, 0f, target.transform.position.z));
            s_dis = Vector3.Distance(target.transform.position, Player.transform.position);
            d_dis = Vector3.Distance(new Vector3(LockOnSystem._SelectLockOnTarget.transform.position.x, 0f, LockOnSystem._SelectLockOnTarget.transform.position.z),new Vector3(Player.position.x, 0f, Player.position.z));

            if (t_dis > TargetAcculate)
            {
                if (s_dis < 100f && d_dis > s_dis)
                {
                    TargetUpServerRpc(new Vector3(LockOnSystem._SelectLockOnTarget.transform.position.x,0f,LockOnSystem._SelectLockOnTarget.transform.position.z));
                }
                else if (s_dis > 40f && d_dis < s_dis)
                {
                    TargetDownServerRpc(new Vector3(LockOnSystem._SelectLockOnTarget.transform.position.x,0f,LockOnSystem._SelectLockOnTarget.transform.position.z));

                }
            }
        }
    }

    [ServerRpc]
    void LaunchServerRpc()
    {
        LaunchClientRpc();
    }

    [ClientRpc]
    void LaunchClientRpc()
    {
        Launch();
        anima.SetTrigger("isAttack");
    }

    [ServerRpc]
    void TargetUpServerRpc(Vector3 LockOnPos)
    {
        TargetUpClientRpc(LockOnPos);
    }
    [ClientRpc]
    void TargetUpClientRpc(Vector3 LockOnPos)
    {
        target.transform.position = Vector3.Lerp(target.transform.position, LockOnPos,TargetSpeed * Time.deltaTime);
    }

    [ServerRpc]
    void TargetDownServerRpc(Vector3 LockOnPos)
    {
        TargetDownClientRpc(LockOnPos);
    }
    [ClientRpc]
    void TargetDownClientRpc(Vector3 LockOnPos)
    {
        target.transform.position = Vector3.Lerp(target.transform.position, LockOnPos,TargetSpeed * Time.deltaTime);
    }

    void Launch()
    {
        Rigidbody clone = ObjectPoller.ObjPollInstance.SpawnFromPool("Projectile1", startPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        clone.GetComponent<ProjectileBullet>().OwnerTag = Player.gameObject.tag;

        if(IsLocalPlayer){
            clone.gameObject.layer = 13;
        }
        else{
            clone.gameObject.layer = 14;
        }

        Physics.gravity = Vector3.up * gravity;
        Invoke("ResetGravity", 1f);

        clone.velocity = CalculateLaunchData().initialVelocity;
    }

    LaunchData CalculateLaunchData()
    {
        float displacementY = target.position.y - startPoint.position.y;
        Vector3 displacementXZ = new Vector3(target.position.x - startPoint.position.x, 0, target.position.z - startPoint.position.z);
        float value = (displacementY - curveHight) / gravity;
        float clampedCurve = Mathf.Clamp(value, 0, value);
        float time = Mathf.Sqrt(-2 * curveHight / gravity) + Mathf.Sqrt(2 * clampedCurve);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * curveHight);
        Vector3 velocityXZ = displacementXZ / time;
        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    void DrawPath()
    {
        LaunchData launchData = CalculateLaunchData();
        Vector3 previousDrawPoint = startPoint.position;


        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = startPoint.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);

            previousDrawPoint = drawPoint;

            lineRenderer.positionCount = resolution;

            lineRenderer.SetPosition(i - 1, drawPoint);
        }
    }

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

    }
    public GameObject BulletSpawnPoint;

    void ResetGravity()
    {
        Physics.gravity = def_Gravity;
    }

    void ResetFire()
    {
        isFire = false;
    }
}
