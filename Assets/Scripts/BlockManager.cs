using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

	private List<Block> blockList;
	public int numInitialBlocks;
	public float minAcceptableDistance;
	public Vector2 range;
	public GameObject blockPrefab;
	public int maxNumTries;

	void Start(){
		blockList = new List<Block> ();
		GenerateInitialBlockSetup ();
	}

	Block Create(Vector3 location){
		GameObject obj = Instantiate (blockPrefab, location, Quaternion.identity) as GameObject;
		Block block = obj.GetComponent<Block> ();
		blockList.Add (block);
		return block;
	}

	Vector3 GenerateLocation(){
		float x = Random.Range (-range.x, range.x);
		float y = Random.Range (-range.y, range.y);
		return new Vector3 (x, y, 0);
	}

	bool ValidateLocation(Vector3 location){
		bool valid = true;
		foreach (Block block in blockList) {
			if (Vector3.Distance (block.transform.position, location) < minAcceptableDistance) {
				valid = false;
				break;
			}
		}
		return valid;
	}

	Vector3 GenerateValidLocation(){
		Vector3 location = GenerateLocation ();
		bool valid = ValidateLocation (location);
		for(int i = 0; i < maxNumTries; i++) {
			if (!valid) {
				location = GenerateLocation ();
				valid = ValidateLocation (location);
			} else {
				break;
			}
		}
		if (!valid) {
			location = Vector3.forward;
		}
		return location;
	}

	Block GenerateValidBlock(){
		Vector3 location = GenerateValidLocation ();
		Block block;
		block = Create (location);
		return block;
	}

	void GenerateInitialBlockSetup(){
		Block block;
		for (int i = 0; i < numInitialBlocks; i++) {
			block = GenerateValidBlock ();
			if (block.transform.position == Vector3.forward) {
				Destroy (block.gameObject);
				Debug.Log("only made " + i + "blocks");
				break;
			}
		}
	}
}
