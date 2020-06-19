using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStat
{
    public string name;
	public int health;
	public int maxHealth;
	public int damage;
	public float attackRange;
	
	public PlayerStat() {
		name = "";
		health = 0;
		maxHealth = 0;
		damage = 0;
		attackRange = 0f;
	}
}
