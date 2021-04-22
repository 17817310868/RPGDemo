using UnityEngine;
using System.Collections;

public class NetworkTransform : MonoBehaviour
{
    public string Name;
    public Vector3 position;
    public Quaternion rotation;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.5f);
        transform.position = Vector3.Lerp(transform.position, position, 1f);
    }
}
