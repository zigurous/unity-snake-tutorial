using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SnakeSegment : MonoBehaviour
{
    private static Dictionary<Vector2Int, Dictionary<Vector2Int, float>> orientations;

    private SpriteRenderer spriteRenderer;

    public Sprite head;
    public Sprite tail;
    public Sprite body;
    public Sprite corner;

    public Vector2Int direction { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetDirection(Vector2Int direction, Vector2Int previous)
    {
        // Initialize the orientations for the first time if not yet set
        if (orientations == null) {
            InitializeOrientations();
        }

        // Calculate the correct rotation based on the direction (new and old)
        float angle = orientations[direction][previous];
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Set the new direction
        this.direction = direction;
    }

    public void Follow(SnakeSegment other, int index, int length)
    {
        // Determine if the segment is the head, tail, or a turning segment
        bool isHead = index == 0;
        bool isTail = index == length - 1;
        bool isTurning = direction != other.direction;

        // Set the correct sprite depending where the segment is in the snake
        if (index == 0) {
            spriteRenderer.sprite = head;
        } else if (index == length - 1) {
            spriteRenderer.sprite = tail;
        } else if (isTurning) {
            spriteRenderer.sprite = corner;
        } else {
            spriteRenderer.sprite = body;
        }

        // The head and tail segments should never be considered turning since
        // the rotation would not match up to the corner pieces
        if (isTurning && !isHead && !isTail) {
            SetDirection(other.direction, direction);
        } else {
            SetDirection(other.direction, Vector2Int.zero);
        }

        // Match the position of the segment to the one its following
        transform.position = other.transform.position;
    }

    private void InitializeOrientations()
    {
        orientations = new Dictionary<Vector2Int, Dictionary<Vector2Int, float>>(5)
        {
            { Vector2Int.zero, new Dictionary<Vector2Int, float>(5) },
            { Vector2Int.right, new Dictionary<Vector2Int, float>(5) },
            { Vector2Int.up, new Dictionary<Vector2Int, float>(5) },
            { Vector2Int.left, new Dictionary<Vector2Int, float>(5) },
            { Vector2Int.down, new Dictionary<Vector2Int, float>(5) }
        };

        orientations[Vector2Int.zero].Add(Vector2Int.zero, 0.0f);
        orientations[Vector2Int.zero].Add(Vector2Int.right, 0.0f);
        orientations[Vector2Int.zero].Add(Vector2Int.up, 90.0f);
        orientations[Vector2Int.zero].Add(Vector2Int.left, 180.0f);
        orientations[Vector2Int.zero].Add(Vector2Int.down, 270.0f);

        orientations[Vector2Int.right].Add(Vector2Int.zero, 0.0f);
        orientations[Vector2Int.right].Add(Vector2Int.left, 0.0f);
        orientations[Vector2Int.right].Add(Vector2Int.right, 0.0f);
        orientations[Vector2Int.right].Add(Vector2Int.down, 0.0f);
        orientations[Vector2Int.right].Add(Vector2Int.up, -90.0f);

        orientations[Vector2Int.up].Add(Vector2Int.zero, 90.0f);
        orientations[Vector2Int.up].Add(Vector2Int.up, 90.0f);
        orientations[Vector2Int.up].Add(Vector2Int.down, 90.0f);
        orientations[Vector2Int.up].Add(Vector2Int.right, 90.0f);
        orientations[Vector2Int.up].Add(Vector2Int.left, 0.0f);

        orientations[Vector2Int.left].Add(Vector2Int.zero, 180.0f);
        orientations[Vector2Int.left].Add(Vector2Int.left, 180.0f);
        orientations[Vector2Int.left].Add(Vector2Int.right, 180.0f);
        orientations[Vector2Int.left].Add(Vector2Int.up, 180.0f);
        orientations[Vector2Int.left].Add(Vector2Int.down, 90.0f);

        orientations[Vector2Int.down].Add(Vector2Int.zero, 270.0f);
        orientations[Vector2Int.down].Add(Vector2Int.down, 270.0f);
        orientations[Vector2Int.down].Add(Vector2Int.up, 270.0f);
        orientations[Vector2Int.down].Add(Vector2Int.left, 270.0f);
        orientations[Vector2Int.down].Add(Vector2Int.right, 180.0f);
    }

}
