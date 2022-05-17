using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private CountDown countDown;
    [SerializeField]
    private MoleSpawner moleSpawner;

    private int score;
    public int Score{
        set=>score = Mathf.Max(0,value);
        get=>score;
    }

    private void Start() {
        countDown.StartCountDown(GameStart);
    }

    private void GameStart(){
        moleSpawner.SetUp();
    }

}