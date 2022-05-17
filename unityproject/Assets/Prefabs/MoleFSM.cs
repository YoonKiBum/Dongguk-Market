using System.Collections;
using UnityEngine;

public enum MoleState {UnderGround = 0, OnGround, MoveUp, MoveDown}

public class MoleFSM : MonoBehaviour
{
    [SerializeField]
    private float waitTimeOnGround;
    [SerializeField]
    private float limitMinY;
    [SerializeField]
    private float limitMaxY;

    private Movement3D movement3D;

    public MoleState MoleState{private set; get;}

    private void Awake() {
        movement3D = GetComponent<Movement3D>();
        ChangeState(MoleState.UnderGround);
    }

    public void ChangeState(MoleState newState){
        StopCoroutine(MoleState.ToString());
        MoleState = newState;
        StartCoroutine(MoleState.ToString());
    }

    private IEnumerator UnderGround(){
        movement3D.MoveTo(Vector3.zero);
        transform.position = new Vector3(transform.position.x, limitMinY, transform.position.z);

        yield return null;
    }

    private IEnumerator OnGround(){
        movement3D.MoveTo(Vector3.zero);
        transform.position = new Vector3(transform.position.x, limitMaxY, transform.position.z);

        yield return new WaitForSeconds(waitTimeOnGround);
    
        ChangeState(MoleState.MoveDown);

    }

    private IEnumerator MoveUp(){
        movement3D.MoveTo(Vector3.up);
        while(true){
            if(transform.position.y >= limitMaxY)
            {
                ChangeState(MoleState.OnGround);
            }
            yield return null;
        }
    }

    private IEnumerator MoveDown(){
        movement3D.MoveTo(Vector3.down);

        while(true){
            if(transform.position.y <= limitMinY)
            {
                ChangeState(MoleState.UnderGround);
            }
            yield return null;
        }
    }

}
