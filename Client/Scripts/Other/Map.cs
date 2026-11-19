using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class Map : MonoBehaviour
{
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 7.0f, player.transform.position.z);
        transform.LookAt(player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }
}
