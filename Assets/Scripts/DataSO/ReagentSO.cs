using UnityEngine;

[CreateAssetMenu(fileName = "ReagentSO", menuName = "Bio-Lab/ReagentSO")]
public class ReagentSO : ScriptableObject
{
    public string reagentName;
    public float volume;            // 持有容量,拥有回调检查是否超量等
    public Color color;
    // public GameObject prefab;       // 溶液的模拟
}