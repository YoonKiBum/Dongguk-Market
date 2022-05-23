using System.Collections;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField]
    private float maxY;
    [SerializeField]
    private float minY;
    [SerializeField]
    private GameObject moleHitEffectPrefab;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private ObjectDetector objectDetector;
    private Movement3D movement3D;

    private void Awake() {
        movement3D = GetComponent<Movement3D>();
        objectDetector.raycastEvent.AddListener(OnHit);
    }

    private void OnHit(Transform target){
        if (target.CompareTag("Mole")){
            MoleFSM mole = target.GetComponent<MoleFSM>();

            if(mole.MoleState == MoleState.UnderGround) return;

            transform.position = new Vector3(target.position.x, minY, target.position.z);

            mole.ChangeState(MoleState.UnderGround);

            //camera shake
            ShakeCamera.Instance.OnShakeCamera(0.1f,0.1f);

            //hit effect
            GameObject clone = Instantiate(moleHitEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = clone.GetComponent<ParticleSystem>().main;
            main.startColor = mole.GetComponent<MeshRenderer>().material.color;

            //score up
            gameController.Score += 50;


            StartCoroutine("MoveUp");

        }
    }

    private IEnumerator MoveUp(){
        movement3D.MoveTo(Vector3.up);

        while(true){
            if (transform.position.y >= maxY){
                movement3D.MoveTo(Vector3.zero);
                break;
            }
            yield return null;
        }
    }


}
