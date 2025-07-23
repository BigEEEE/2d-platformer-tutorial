using UnityEngine;

public class PickUpDoubleJumpUpgrade : MonoBehaviour, Item
{
    public GameObject player;
    public void Collect()
    {
        player.GetComponent<PlayerMovement>().jumpLimit = 2;
        Destroy(gameObject);
    }
}
