using UnityEngine;

public class FishController : MonoBehaviour
{
    public string fishType;
    public float strength;
    public int value;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float diagonalMovementDuration;
    [SerializeField] private float diagonalMovementDelay;
    [SerializeField] private float maxRotation;
    [SerializeField] private Vector2 movementRange;
    public bool dead;
    public bool falling;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        //Randomizza la direzione del movimento
    }

    private void OnDisable()
    {
        //Riattiva collider e script
    }

    private void Move()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Collisione con l'amo
    }
}
