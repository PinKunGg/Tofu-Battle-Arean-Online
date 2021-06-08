using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCore : MonoBehaviour
{
    public float MaxHp;
    public Slider _HpBar;

    private void Start() {
        _HpBar.maxValue = MaxHp;
        _HpBar.value = MaxHp;
    }

    public void TakeDamage(float damage){
        _HpBar.value -= damage;
        if(_HpBar.value <= 0){
            Debug.Log("Die");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("PlayerBullet")){
            TakeDamage(5f);
            other.gameObject.SetActive(false);
        }
    }
}
