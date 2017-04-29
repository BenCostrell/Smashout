using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Prefab DB")]
public class PrefabDB : ScriptableObject {
	[SerializeField]
	private GameObject player;
	public GameObject Player { get { return player; } }

    [SerializeField]
    private GameObject reticle;
    public GameObject Reticle { get { return reticle; } }

    [SerializeField]
    private GameObject hitShockwave;
    public GameObject HitShockwave { get { return hitShockwave; } }

    [SerializeField]
    private GameObject deathExplosion;
    public GameObject DeathExplosion { get { return deathExplosion; } }
}
