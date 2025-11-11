using UnityEngine;

public class NPCIndicator : MonoBehaviour
{
    public Canvas canvas;      // Assign the World Space Canvas
    public Transform player;   // Assign Player or Main Camera
    public float showDistance = 10f; // Distance to show indicator

    void Update()
    {
        if (!player || !canvas) return;

        // Distance check
        float distance = Vector3.Distance(player.position, transform.position);
        canvas.enabled = distance <= showDistance;

        // Face the camera/player
        Vector3 lookPos = player.position - canvas.transform.position;
        lookPos.y = 0; // optional: keep indicator upright
        canvas.transform.rotation = Quaternion.LookRotation(-lookPos);
    }

    // Optional helper to manually toggle
    public void Show(bool show)
    {
        canvas.enabled = show;
    }
}
