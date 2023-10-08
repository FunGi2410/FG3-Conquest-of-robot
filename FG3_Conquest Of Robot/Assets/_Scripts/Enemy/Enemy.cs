using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : LivingEntity
{
    [SerializeField]
    protected Enemy_SO enemy_SO;

    protected Rigidbody2D mRig;
    public enum State { Idle, Chasing, Attacking };
    State currentState;

    
    Transform target;

    LivingEntity targetEntity;

    GunController gunController;

    float attackDistance;

    bool hasTarget;

    public Animator anim;
   
    private bool moving;
    
    Vector2 dirToTarget;
    float dirAnim;

    float timmer = 0f;
    float timeIdleToAttack = 1f;

    protected override void Start()
    {
        base.Start();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            this.hasTarget = true;

            this.target = GameObject.FindGameObjectWithTag("Player").transform;
            this.targetEntity = this.target.GetComponent<LivingEntity>();
            this.targetEntity.OnDeath += this.OnTargetDeath;
        }

        this.gunController = GetComponent<GunController>();

        this.mRig = GetComponent<Rigidbody2D>();

        this.SetProperties();

        // Animate
        this.anim = GetComponent<Animator>();
    }

    void SetProperties()
    {
        // health
        this.startingHealth = this.enemy_SO.health;
        // disAttack
        this.attackDistance = this.enemy_SO.disAttack;
        // Gun
        this.gunController.startingGun = this.enemy_SO.gun;
        this.gunController.EquipGun(this.gunController.startingGun);
    }

    private void Update()
    {
        if (this.hasTarget)
        {
            this.Attack();
        }
        this.Animate();
    }

    private void FixedUpdate()
    {
       
        this.MovetoTarget();
    }


    protected virtual void MovetoTarget()
    {
        if (this.target != null)
        {
            float disToTarget = Vector2.Distance(transform.position, this.target.position);
            if (disToTarget > this.enemy_SO.disAttack && this.timmer == 0)
            {
                this.moving = true;

                this.dirToTarget = (this.target.position - transform.position).normalized;
                Vector2 moveVelocity = this.dirToTarget * this.enemy_SO.speedWalk * Time.fixedDeltaTime;
                this.mRig.MovePosition(this.mRig.position + moveVelocity);

                
            }
            else if (disToTarget < this.enemy_SO.disAttack || this.timmer != 0)
            {
                if(this.timmer < this.timeIdleToAttack)
                {
                    this.moving = false;
                    this.mRig.velocity = Vector2.zero;

                    this.timmer += Time.fixedDeltaTime;
                }
                else
                {
                    this.timmer = 0f;
                }
            }
            
        }
    }

    private void Animate()
    {
        this.anim.SetBool("Moving", this.moving);

        if (this.dirToTarget.x < 0) this.dirAnim = -1f;
        else if (this.dirToTarget.x > 0) this.dirAnim = 1f;
        this.anim.SetFloat("X", this.dirAnim);

        this.anim.SetBool("Moving", this.moving);
    }

    void OnTargetDeath()
    {
        this.hasTarget = false;
    }

    protected override void Die()
    {
        this.OnDeath += FindObjectOfType<Spawner>().OneEnemyDeath;
        base.Die();
    }

    private void Attack()
    {
        // calculate distance to player
        float sqrDisToTarget = (this.target.position - transform.position).sqrMagnitude;
        // rotate weapon 
        if (sqrDisToTarget <= Mathf.Pow(this.attackDistance, 2))
        {
            Vector2 mPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPos = new Vector2(this.target.position.x, this.target.position.y);
            Vector2 lookDir = targetPos - mPos;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            //transform.eulerAngles = new Vector3(0, 0, angle);
            transform.GetChild(0).gameObject.transform.localEulerAngles = new Vector3(0, 0, angle);
            
            // shoot
            this.gunController.Shoot();
        }
        else
        {
            return;
        }
    }
}
