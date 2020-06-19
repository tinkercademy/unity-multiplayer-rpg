using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class CharacterStats : ScriptableObject
{
	public List<PlayerStat> statList;
}

/*[System.Serializable]
public class PlayerStat
{
	public string name;
	public int health;
	public int damage;
	public int attackRange;
    //public List<string> stat;
}*/

/*[System.Serializable]
public class PlayerStatList
{
    public List<PlayerStat> statList;
}*/