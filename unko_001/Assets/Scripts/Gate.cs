using UnityEngine;
using TMPro;

public enum OperationType
{
    Add,
    Subtract,
    Multiply,
    Divide
}

public class Gate : MonoBehaviour
{
    public OperationType operation;
    public float operand;
    public TextMeshPro label;
    public MeshRenderer meshRenderer;

    private bool used = false;

    public void Initialize(OperationType op, float value, Material goodMat, Material badMat)
    {
        operation = op;
        operand = value;

        // Set label text
        string prefix = "";
        switch (op)
        {
            case OperationType.Add: prefix = "+"; break;
            case OperationType.Subtract: prefix = "-"; break;
            case OperationType.Multiply: prefix = "×"; break;
            case OperationType.Divide: prefix = "÷"; break;
        }
        if (label != null)
            label.text = prefix + Mathf.RoundToInt(value).ToString();

        // Set color based on operation type
        bool isGood = (op == OperationType.Add || op == OperationType.Multiply);
        if (meshRenderer != null)
            meshRenderer.material = isGood ? goodMat : badMat;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ApplyOperation(operation, operand);
            used = true;

            // Disable the entire gate pair
            if (transform.parent != null)
                transform.parent.gameObject.SetActive(false);
            else
                gameObject.SetActive(false);
        }
    }
}
