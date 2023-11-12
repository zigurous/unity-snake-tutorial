using UnityEngine;

/// <summary>
/// Manages the state of the game, such as scoring
/// losing, and starting a new game.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The area marked by the collider that is
    /// considered in bounds.
    /// </summary>
    [Tooltip("The area marked by the collider that is considered in bounds.")]
    public Collider2D gridArea;

    /// <summary>
    /// The snake component.
    /// </summary>
    [Tooltip("The snake component.")]
    public Snake snake;

    /// <summary>
    /// The food game object.
    /// </summary>
    [Tooltip("The food game object.")]
    public GameObject food;

    /// <summary>
    /// The player's current score.
    /// </summary>
    [HideInInspector]
    public int score;

    private void Start()
    {
        NewGame();
    }

    public void GameOver()
    {
        // Start a new game immediately after losing
        NewGame();
    }

    public void NewGame()
    {
        // Reset the score
        this.score = 0;

        // Reset the snake's size, position, and direction
        this.snake.ResetSize();
        this.snake.direction = Vector2.right;
        this.snake.transform.position = Vector3.zero;

        // Give the food an initial random position
        RandomizeFoodPosition();
    }

    public void FoodEaten()
    {
        // Grow the snake and increase the score
        this.snake.Grow();
        this.score++;

        // Move the food to a new position
        RandomizeFoodPosition();
    }

    public void RandomizeFoodPosition()
    {
        Bounds bounds = this.gridArea.bounds;

        // Pick a random position inside the bounds
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        // Set the food position but round the values
        // to ensure it aligns with the grid
        this.food.transform.position = new Vector2(Mathf.Round(x), Mathf.Round(y));
    }

}
