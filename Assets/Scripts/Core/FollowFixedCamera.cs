using UnityEngine;

public class FollowFixedCamera : MonoBehaviour
{
    [SerializeField] private Vector3 fixedPosition = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        transform.position = fixedPosition;
    }
}
