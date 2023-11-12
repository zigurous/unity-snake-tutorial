using UnityEngine;

/// <summary>
/// Handles setting the position of the food.
/// </summary>
public class Food : MonoBehaviour
{
    /// <summary>
    /// The area marked by the collider that is considered in bounds.
    /// </summary>
    [Tooltip("The area marked by the collider that is considered in bounds.")]
    public Collider2D gridArea;

    private void Start()
    {
        // Give the food an initial random position
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        Bounds bounds = this.gridArea.bounds;

        // Pick a random position inside the bounds
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        // Set the food position but round the values to ensure it aligns with
        // the grid
        this.transform.position = new Vector2(Mathf.Round(x), Mathf.Round(y));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Move the food to a new position when the snake eats it
        RandomizePosition();
    }

}
