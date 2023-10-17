using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenPlankController : MonoBehaviour
{
    readonly Vector3 offsetHorizontal = new(-32.9f, 0.2523417f, -0f);
    readonly Vector3 offsetVertical = new(0f, 32.65f, 0f);

    readonly float widthPlank = 65f;

    [SerializeField] Direction direction;
    [SerializeField] int totalPlank;
    [SerializeField] GameObject plank;

    enum Direction
    {
        Left, Top
    }

    // Start is called before the first frame update
    void Start()
    {
        GeneratePlank();
    }

    void GeneratePlank()
    {
        switch (direction)
        {
            case Direction.Left:
                LoopGenerate(Vector3.left, Quaternion.identity, true);
                break;
            case Direction.Top:
                LoopGenerate(Vector3.up, Quaternion.Euler(0f, 0f, 90f), false);
                break;
        }
    }

    void LoopGenerate(Vector3 directionPos, Quaternion rot, bool isHorizontal)
    {
        Vector3 offset = isHorizontal ? offsetHorizontal : offsetVertical;

        for (int i = 0; i < totalPlank; i++)
        {
            Vector3 pos = i * widthPlank * directionPos + offset;
            GameObject item = Instantiate(plank, transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = pos;
            item.transform.localRotation = rot;
        }
    }
}

