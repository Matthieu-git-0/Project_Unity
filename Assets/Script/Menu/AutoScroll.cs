using UnityEngine;

public class AutoScroll : MonoBehaviour
{
    public float scrollSpeed = 40f;

    void Update()
    {
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
        
        if (transform.localPosition.y > 1830)
        {
            transform.localPosition = new Vector3(0, -500, 0);
        }
    }
}