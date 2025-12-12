using UnityEngine;

public class Explosionn : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        Enemy enemy = collision.GetComponent<Enemy>();
        if (collision.CompareTag("Player"))
        {
            player.TakeDame(damage);
        }
        if (collision.CompareTag("Enemy"))
        {
            enemy.TakeDame(damage);
        }
    }
    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}
