using UnityEngine;
using UniRx;
using System.Collections;
using System.Collections.Generic;

public class ExplosionObject : MonoBehaviour {
	[SerializeField] GameObject explosionPrefab;
	List<GameObject> explode;

	void Start(){
		explode = new List<GameObject>();
	}

	public void Explosion(string cmd){
		Debug.Log ("explosion");
		explode.Add(Instantiate (explosionPrefab) as GameObject);
		explode[explode.Count-1].transform.SetParent (this.transform);
		explode[explode.Count-1].transform.position = new Vector3 (
			this.transform.position.x,
			this.transform.position.y,
			this.transform.position.z);
		Observable.FromCoroutine(DeleteExplosion).Subscribe ().AddTo(this);
	}

	IEnumerator DeleteExplosion() {
		Debug.Log ("destroy explosion");
		yield return new WaitForSeconds(2);
		Destroy (explode[0].gameObject);
		explode.RemoveAt (0);
	}
}
