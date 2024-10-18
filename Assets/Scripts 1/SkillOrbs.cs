using UnityEngine;

public class SkillOrbs : MonoBehaviour
{
    private Vector3 originalScale; // To store the original scale of the orb
    [SerializeField] private Vector3 hoverScale = new Vector3(0.35f, 0.35f, 0.35f);

    // Enum to define different skill types
    public enum SkillType
    {
        Damage,
        Debuff,
        Block
    }
    [SerializeField] private SkillType skillType;

    //Enemy reference
    [SerializeField] private GameObject enemyObject;

    // Start is called before the first frame update
    void Start()
    {
        // Store the original scale of the orb
        originalScale = transform.localScale;
        // Automatically find theenemy object by tag
        enemyObject = GameObject.FindWithTag("Enemy");
    }

    // This method will be called from the player's script when the orb is hovered
    public void OnHover(bool isHovering)
    {
        if (isHovering)
        {
            // Scale up the orb when the mouse hovers over it
            transform.localScale = hoverScale;
        }
        else
        {
            // Scale the orb back to its original size when the mouse is not hovering
            transform.localScale = originalScale;
        }
    }
    // This method will be called when the player clicks on the orb to use its skill
    public void ActivateSkill()
    {

        switch (skillType)
        {
            case SkillType.Damage:
                DealDamage();
                break;
            case SkillType.Debuff:
                ApplyDebuff();
                break;
            case SkillType.Block:
                ActivateShield();
                break;
        }
    }


    private void DealDamage()
    {
        float damage = 30f;
        Debug.Log("DealDamage skill activated!");
        enemyObject.GetComponent<EnemyTest>().TakeDamage(damage);
    }

    private void ApplyDebuff()
    {

        Debug.Log("ApplyDebuff skill activated!");
        enemyObject.GetComponent<EnemyTest>().TakeDebuff(30f);

    }
     private void ActivateShield(){
        Debug.Log("ActivateShield skill activated!");
     }
}
