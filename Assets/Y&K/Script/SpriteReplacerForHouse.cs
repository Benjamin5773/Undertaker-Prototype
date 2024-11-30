using UnityEngine;

public class SpriteReplacerForHouse : MonoBehaviour
{  [Header("Player Setting")]
    [Tooltip("Tag of the player object to detect")]
    [SerializeField] private string playerTag = "Player";

    [Header("Objects and Sprites")]
    [Tooltip("Each GameObject and its corresponding new Sprite")]
    [SerializeField] private SpriteChange[] spriteChanges;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true;
            ReplaceSprites();
        }
    }

    private void ReplaceSprites()
    {
        if (spriteChanges == null || spriteChanges.Length == 0)
        {
            Debug.LogWarning("No objects or sprites assigned!");
            return;
        }

        foreach (SpriteChange change in spriteChanges)
        {
            if (change.obj != null && change.newSprite != null)
            {
                SpriteRenderer spriteRenderer = change.obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = change.newSprite;
                }
                else
                {
                    Debug.LogWarning($"GameObject {change.obj.name} does not have a SpriteRenderer component!");
                }
            }
            else
            {
                Debug.LogWarning("One of the objects or sprites is null!");
            }
        }
    }

    [System.Serializable]
    public class SpriteChange
    {
        public GameObject obj;
        public Sprite newSprite;
    }
}
