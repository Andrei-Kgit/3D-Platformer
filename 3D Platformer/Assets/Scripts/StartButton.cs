using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private RunTimer _runTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _runTimer.StartRun();
        }
    }
}
