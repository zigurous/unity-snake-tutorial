using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;
    public Vector2 direction = Vector2.right;
    public int initialSize = 4;

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            this.direction = Vector2.up;
        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            this.direction = Vector2.down;
        } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            this.direction = Vector2.right;
        } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            this.direction = Vector2.left;
        } else {
            this.direction = Vector2.zero;
        }

        // Only move the snake during the frame an input was pressed
        if (this.direction != Vector2.zero)
        {
            // Set each segment's position to be the same as the one it follows.
            // We must do this in reverse order so the position is set to the
            // previous position, otherwise they will all be stacked on top of
            // each other.
            for (int i = _segments.Count - 1; i > 0; i--) {
                _segments[i].position = _segments[i - 1].position;
            }

            // Move the snake in the direction it is facing
            // Round the values to ensure it aligns to the grid
            this.transform.position = new Vector3(
                Mathf.Round(this.transform.position.x) + this.direction.x,
                Mathf.Round(this.transform.position.y) + this.direction.y);
        }
    }

    public void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    public void ResetState()
    {
        this.direction = Vector2.right;
        this.transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < _segments.Count; i++) {
            Destroy(_segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        _segments.Clear();
        _segments.Add(this.transform);

        // -1 since the head is already in the list
        for (int i = 0; i < this.initialSize - 1; i++) {
            Grow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food") {
            Grow();
        } else if (other.tag == "Obstacle") {
            ResetState();
        }
    }

}
