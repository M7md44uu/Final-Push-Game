using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float time;
    void Start()
    {
        Destroy(gameObject, time);
    }


}
