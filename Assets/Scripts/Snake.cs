using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement of the snake and growing in size.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    /// <summary>
    /// The list of segments of the snake.
    /// </summary>
    private List<Transform> _segments = new List<Transform>();

    /// <summary>
    /// The object that is cloned when creating a new segment to grow the snake.
    /// </summary>
    [Tooltip("The object that is cloned when creating a new segment to grow the snake.")]
    public Transform segmentPrefab;

    /// <summary>
    /// The number of segments the snake has initially.
    /// </summary>
    [Tooltip("The number of segments the snake has initially.")]
    public int initialSize = 4;

    /// <summary>
    /// The direction the snake is moving.
    /// </summary>
    [HideInInspector]
    public Vector2 direction = Vector2.right;

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // If moving horizontal, then only allow turning up or down
        if (this.direction.x != 0.0f)
        {
            // Set the direction based on the input key being pressed
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                this.direction = Vector2.up;
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                this.direction = Vector2.down;
            }
        }
        // If moving vertical, then only allow turning left or right
        else if (this.direction.y != 0.0f)
        {
            // Set the direction based on the input key being pressed
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                this.direction = Vector2.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                this.direction = Vector2.left;
            }
        }
    }

    private void FixedUpdate()
    {
        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = _segments.Count - 1; i > 0; i--) {
            _segments[i].position = _segments[i - 1].position;
        }

        // Increase the snake's position by one in the direction they are
        // moving. Round the position to ensure it stays aligned to the grid.
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + this.direction.x,
            Mathf.Round(this.transform.position.y) + this.direction.y);
    }

    public void Grow()
    {
        // Create a new segment
        Transform segment = Instantiate(this.segmentPrefab);

        // Position the segment at the same spot as the last segment
        segment.position = _segments[_segments.Count - 1].position;

        // Add the segment to the end of the list
        _segments.Add(segment);
    }

    public void ResetState()
    {
        // Set the initial direction of the snake, starting at the origin
        // (center of the grid)
        this.direction = Vector2.right;
        this.transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < _segments.Count; i++) {
            Destroy(_segments[i].gameObject);
        }

        // Clear the list then add the head (this) as the first segment
        _segments.Clear();
        _segments.Add(this.transform);

        // Grow the snake to the initial size -1 since the head was already
        // added
        for (int i = 0; i < this.initialSize - 1; i++) {
            Grow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            // Food eaten, increase the size of the snake
            Grow();
        }
        else if (other.tag == "Obstacle")
        {
            // Game over, reset the state of the snake
            ResetState();
        }
    }

}
