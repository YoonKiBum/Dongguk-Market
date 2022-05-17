using System.Collections;
using UnityEngine;

public class MoleSpawner : MonoBehaviour
{
    [SerializeField]
    private MoleFSM[] moles;
    [SerializeField]
    private float spawnTime;

    private void Start() {
        StartCoroutine("SpawnMole");

    }

    private IEnumerator SpawnMole(){
        while(true){
            int index = Random.Range(0, moles.Length);
            moles[index].ChangeState(MoleState.MoveUp);

            yield return new WaitForSeconds(spawnTime);
        }
    }

}
