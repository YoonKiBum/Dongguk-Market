using System.Collections;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private static ShakeCamera instance;
    public static ShakeCamera Instance => instance;

    private float shakeTime;
    private float shakeIntensity;

    public ShakeCamera(){
        instance = this;
    }

    public void OnShakeCamera(float shakeTime = 1.0f, float shakeIntensity = 0.1f){
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;

        StopCoroutine("ShakeByPosition");
        StartCoroutine("ShakeByPosition");
    }

    private IEnumerator ShakeByPosition(){
        Vector3 startPosition = transform.position;
        while(shakeTime > 0.0f){
            transform.position = startPosition + Random.insideUnitSphere*shakeIntensity;
            shakeTime -=Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition;
    }

}
