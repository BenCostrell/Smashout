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
	public float blockDeathTime;
	public float blockAppearanceTime;

	void Start(){
	}

	Block Create(Vector3 location){
		GameObject obj = Instantiate (blockPrefab, location, Quaternion.identity) as GameObject;
		Block block = obj.GetComponent<Block> ();
		block.GetComponent<SpriteRenderer> ().enabled = false;
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

	public void GenerateInitialBlockSetup(){
		blockList = new List<Block> ();
		Block block;
		for (int i = 0; i < numInitialBlocks; i++) {
			block = GenerateValidBlock ();
			if (block.transform.position == Vector3.forward) {
				Destroy (block.gameObject);
				Debug.Log ("only made " + i + "blocks");
				break;
			} else {
				blockList.Add (block);
			}
		}
	}

	public void DestroyBlock(Block block){
		Collider2D[] colliders = block.GetComponentsInChildren<Collider2D> ();
		foreach (Collider2D col in colliders) {
			col.enabled = false;
		}
		block.gameObject.GetComponent<SpriteRenderer> ().color = Color.red;
		block.StartDestructionAnimation ();
		blockList.Remove (block);
		Destroy (block.gameObject, blockDeathTime);
	}

	public void DestroyAllBlocks(){
		for (int i = blockList.Count - 1; i >= 0; i--) {
			Block block = blockList [i];
			DestroyBlock (block);
		}
	}

	public void StartAppearanceOfAllBlocks(){
		foreach (Block block in blockList) {
			block.StartAppearanceAnimation ();
		}
	}

}
