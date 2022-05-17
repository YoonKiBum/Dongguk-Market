using UnityEngine;

public class GameController : MonoBehaviour
{
    private int score;
    public int Score{
        set=>score = Mathf.Max(0,value);
        get=>score;
    }
}