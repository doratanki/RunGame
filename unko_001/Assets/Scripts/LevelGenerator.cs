using UnityEngine;
using TMPro;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefab References (set at runtime)")]
    public Material groundMat;
    public Material gateGoodMat;
    public Material gateBadMat;
    public Material bossMat;

    [Header("Level Settings")]
    public int gateCount = 8;
    public float gateInterval = 20f;
    public float groundSegmentLength = 20f;
    public float laneDistance = 2f;

    private Transform levelParent;

    public void GenerateLevel()
    {
        // Clean up old level
        if (levelParent != null)
            Destroy(levelParent.gameObject);

        levelParent = new GameObject("Level").transform;

        float totalLength = gateCount * gateInterval + 60f;
        int segmentCount = Mathf.CeilToInt(totalLength / groundSegmentLength);

        // Spawn ground segments
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground_" + i;
            ground.transform.parent = levelParent;
            ground.transform.localScale = new Vector3(laneDistance * 3f + 2f, 0.1f, groundSegmentLength);
            ground.transform.position = new Vector3(0f, -0.55f, i * groundSegmentLength + groundSegmentLength * 0.5f);

            if (groundMat != null)
                ground.GetComponent<MeshRenderer>().material = groundMat;
        }

        // Spawn side walls for visual guide
        for (int i = 0; i < segmentCount; i++)
        {
            float wallX = laneDistance * 1.5f + 1.5f;
            SpawnWall(new Vector3(-wallX, 0.5f, i * groundSegmentLength + groundSegmentLength * 0.5f), groundSegmentLength);
            SpawnWall(new Vector3(wallX, 0.5f, i * groundSegmentLength + groundSegmentLength * 0.5f), groundSegmentLength);
        }

        // Spawn gate pairs
        for (int i = 0; i < gateCount; i++)
        {
            float z = 30f + i * gateInterval;
            SpawnGatePair(z, i);
        }

        // Spawn boss
        float bossZ = 30f + gateCount * gateInterval + 10f;
        SpawnBoss(bossZ);
    }

    void SpawnWall(Vector3 pos, float length)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall";
        wall.transform.parent = levelParent;
        wall.transform.localScale = new Vector3(0.2f, 1f, length);
        wall.transform.position = pos;

        if (groundMat != null)
            wall.GetComponent<MeshRenderer>().material = groundMat;
    }

    void SpawnGatePair(float zPos, int index)
    {
        GameObject pair = new GameObject("GatePair_" + index);
        pair.transform.parent = levelParent;
        pair.transform.position = new Vector3(0f, 0f, zPos);

        // Decide operations: one good, one bad
        OperationType goodOp = Random.value > 0.5f ? OperationType.Add : OperationType.Multiply;
        OperationType badOp = Random.value > 0.5f ? OperationType.Subtract : OperationType.Divide;

        float goodValue, badValue;
        if (goodOp == OperationType.Add)
            goodValue = Random.Range(10f, 80f);
        else
            goodValue = Random.Range(2f, 5f);

        if (badOp == OperationType.Subtract)
            badValue = Random.Range(10f, 50f);
        else
            badValue = Random.Range(2f, 4f);

        // Randomly assign left/right
        bool goodOnLeft = Random.value > 0.5f;
        float leftX = -laneDistance;
        float rightX = laneDistance;

        SpawnGate(pair.transform, goodOnLeft ? leftX : rightX, zPos,
            goodOp, goodValue);
        SpawnGate(pair.transform, goodOnLeft ? rightX : leftX, zPos,
            badOp, badValue);
    }

    void SpawnGate(Transform parent, float xPos, float zPos, OperationType op, float value)
    {
        GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gate.name = "Gate_" + op.ToString();
        gate.transform.parent = parent;
        gate.transform.position = new Vector3(xPos, 1f, zPos);
        gate.transform.localScale = new Vector3(laneDistance * 0.9f, 2.5f, 0.3f);

        // Make trigger
        BoxCollider col = gate.GetComponent<BoxCollider>();
        col.isTrigger = true;
        // Add a thicker trigger zone for reliable detection
        BoxCollider triggerCol = gate.AddComponent<BoxCollider>();
        triggerCol.isTrigger = true;
        triggerCol.size = new Vector3(1f, 1f, 5f);

        // Add Gate script
        Gate gateScript = gate.AddComponent<Gate>();

        // Add label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.parent = gate.transform;
        labelObj.transform.localPosition = new Vector3(0f, 0f, -0.2f);
        labelObj.transform.localScale = Vector3.one * 0.5f;
        TextMeshPro tmp = labelObj.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 8f;
        tmp.color = Color.white;

        gateScript.label = tmp;
        gateScript.meshRenderer = gate.GetComponent<MeshRenderer>();
        gateScript.Initialize(op, value, gateGoodMat, gateBadMat);
    }

    void SpawnBoss(float zPos)
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boss.name = "Boss";
        boss.transform.parent = levelParent;
        boss.transform.position = new Vector3(0f, 1.5f, zPos);
        boss.transform.localScale = new Vector3(laneDistance * 3f, 3f, 1f);

        BoxCollider col = boss.GetComponent<BoxCollider>();
        col.isTrigger = true;

        if (bossMat != null)
            boss.GetComponent<MeshRenderer>().material = bossMat;

        // Add BossZone script
        BossZone bossZone = boss.AddComponent<BossZone>();

        // Add label
        GameObject labelObj = new GameObject("BossLabel");
        labelObj.transform.parent = boss.transform;
        labelObj.transform.localPosition = new Vector3(0f, 0f, -0.6f);
        labelObj.transform.localScale = Vector3.one * 0.3f;
        TextMeshPro tmp = labelObj.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 12f;
        tmp.color = Color.white;

        bossZone.bossNumberText = tmp;

        // Calculate boss number (moderate difficulty)
        float bossNumber = 50f + gateCount * 30f;
        bossZone.Initialize(bossNumber);
    }
}
