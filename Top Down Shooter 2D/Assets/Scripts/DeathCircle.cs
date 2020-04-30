using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCircle : MonoBehaviour
{
    public static DeathCircle instance;

    [SerializeField] Transform targetCircleTransform;
    Transform circleTransform;
    Transform topTransform;
    Transform bottomTransform;
    Transform leftTransform;
    Transform rightTransform;

    [SerializeField] float maxMovement;
    [SerializeField] float minMovement;
    [SerializeField] float shrinkTimer;
    [SerializeField] float circleShrinkSpeed;
    [SerializeField] float circleMoveSpeed;

    bool isMoving;

    [SerializeField] int damagePerSecond;

    [SerializeField] Vector3 initalCircleSize;
    [SerializeField] Vector3 initalCirclePosition;

    Vector3 circleSize;
    Vector3 circlePosition;

    Vector3 targetCircleSize;
    Vector3 targetCirclePosition;

    private void Awake()
    {
        instance = this;

        circleTransform = transform.Find("circle");
        topTransform = transform.Find("top");
        bottomTransform = transform.Find("bottom");
        leftTransform = transform.Find("left");
        rightTransform = transform.Find("right");

        SetCircleSize(initalCirclePosition, initalCircleSize);
        GenerateTargetCircle();
    }

    private void Update()
    {
        shrinkTimer -= Time.deltaTime;

        if (shrinkTimer < 0)
        {
            isMoving = true;

            Vector3 sizeChangeVector = (targetCircleSize - circleSize).normalized;
            Vector3 newCircleSize = circleSize + sizeChangeVector * Time.deltaTime * circleShrinkSpeed;

            Vector3 circleMoveDir = (targetCirclePosition - circlePosition).normalized;
            Vector3 newCirclePosition = circlePosition + circleMoveDir * Time.deltaTime * circleMoveSpeed;

            SetCircleSize(newCirclePosition, newCircleSize);

            float distanceTestAmount = .1f;
            if (Vector3.Distance(newCircleSize, targetCircleSize) < distanceTestAmount && Vector3.Distance(newCirclePosition, targetCirclePosition) < distanceTestAmount)
            {
                print("NEW CIRCLE");
                GenerateTargetCircle();
            }
        }
        else
        {
            isMoving = false;
        }

    }

    void GenerateTargetCircle()
    {
        targetCircleTransform.gameObject.SetActive(true);

        float shrinkSizeAmount = Random.Range(targetCircleTransform.localScale.x / 3.5f, targetCircleTransform.localScale.x / 1.75f);
        Vector3 generatedTargetCircleSize = circleSize - new Vector3(shrinkSizeAmount, shrinkSizeAmount);

        Vector3 movePositionAmount = new Vector2(Random.Range(minMovement, maxMovement), Random.Range(minMovement, maxMovement));
        Vector3 generatedTargetCirclePosition = circlePosition + movePositionAmount;

        float shrinkTime = Random.Range(10, 30);

        SetTargetCircle(generatedTargetCirclePosition, generatedTargetCircleSize, shrinkTime);
    }

    void SetCircleSize(Vector3 position, Vector3 size)
    {
        circlePosition = position;
        circleSize = size;

        transform.position = position;

        circleTransform.localScale = size;

        topTransform.localScale = new Vector3(1500, 1000);
        topTransform.localPosition = new Vector3(0, topTransform.localScale.y / 2 + size.y / 2);

        bottomTransform.localScale = new Vector3(1500, 1000);
        bottomTransform.localPosition = new Vector3(0, -topTransform.localScale.y / 2 - size.y / 2);

        leftTransform.localScale = new Vector3(1000, circleTransform.localScale.y);
        leftTransform.localPosition = new Vector3(-leftTransform.localScale.x / 2 - size.y / 2, 0);

        rightTransform.localScale = new Vector3(1000, circleTransform.localScale.y);
        rightTransform.localPosition = new Vector3(leftTransform.localScale.x / 2 + size.y / 2, 0);

        if (size.x < 1)
        {
            this.enabled = false;
        }
    }

    void SetTargetCircle(Vector3 position, Vector3 size, float shrinkTimer)
    {
        this.shrinkTimer = shrinkTimer;

        targetCircleTransform.position = position;
        targetCircleTransform.localScale = size;

        targetCircleSize = size;
        targetCirclePosition = position;
    }

    bool isInsideSafeZone(Vector3 position)
    {
        return Vector3.Distance(position, circlePosition) < (circleSize.x / 2) - 5;
    }

    bool IsOutsideCircle(Vector3 position)
    {
        return Vector3.Distance(position, circlePosition) > circleSize.x / 2;
    }


    public static bool isInsideSafeZone_Static(Vector3 position)
    {
        return instance.isInsideSafeZone(position);
    }

    public static bool IsOutsideCircle_Static(Vector3 position)
    {
        return instance.IsOutsideCircle(position);
    }
    bool IsMoving()
    {
        return isMoving;
    }

    public static bool IsMoving_Static()
    {
        return instance.IsMoving();
    }
}
