using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Vector3 ScreenToWorld(Camera camera, Vector3 position, float posZ)
    {
        position.z = posZ;
        return camera.ScreenToWorldPoint(position);
    }
}
