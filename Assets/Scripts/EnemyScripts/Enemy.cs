using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Attacker, IDamageable
{
    [SerializeField]
    protected int health;
    public int Health { get => health; set => health = value; }

    [SerializeField] protected float _flashTime = 0.25f;

    private SpriteRenderer _spriteRenderer;
    private Material _material;

    protected EnemyStates currentState = EnemyStates.FlyingToTheScreen;
    protected enum EnemyStates
    {
        FlyingToTheScreen,
        Attacking,
        Leaving
    }

    protected virtual void Awake()
    {
        Init();
    }

    protected void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
        _material.SetFloat("_FlashAmount", 0);
    }
    public void Damage(int damage)
    {
        Health -= damage;
        DamageUI.instance.ShowDamageOnEnemy(transform.position);
        if (Health <= 0) 
        {
            //StopCoroutine(DamageFlasher());
            gameObject.SetActive(false);
        }
        if(gameObject.activeSelf)
                CallDamageFlash();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            Damage(Weapon.Instance.damage);
            collision.gameObject.SetActive(false);
        }
        if (collision.CompareTag("HealingBullet"))
        {
            Damage(-1);
            collision.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fire"))
        {
            Damage(1);
        }
    }

    public void CallDamageFlash()
    {
        StartCoroutine(DamageFlasher());
    }
    protected IEnumerator DamageFlasher()
    {
        if (_material == null || _spriteRenderer == null) Init();
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < _flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / _flashTime));
            _material.SetFloat("_FlashAmount", currentFlashAmount);

            yield return null; 
        }
    }
    
}
