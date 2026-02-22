using System;
using System.Collections.Generic;
using System.Linq;
using PCR.Core;
using PCR.Helper;
using TMPro;
using UnityEngine;

/// <summary>
/// 纯容器逻辑
/// 适用于 烧杯 试管 移液枪等
/// </summary>
public class LiquidContainer : MonoBehaviour
{
    [Serializable]
    public class InitEntry
    {
        public ReagentDataSO reagent;
        public float volume;
    }

    [Header("Config")]
    [SerializeField] private string _containerID;
    [SerializeField] private ContainerType _type;
    [SerializeField] private float _maxVolume = 1000f;

    [SerializeField] private InitEntry initEntry = new();

    private Dictionary<ReagentDataSO, float> _contents = new Dictionary<ReagentDataSO, float>();

    public string ID => _containerID;
    public ContainerType Type => _type;
    public float CurrentVolume { get; private set; }
    public Color CurrentColor { get; private set; } = Color.clear;

    public Dictionary<ReagentDataSO, float> GetContentsSnapshot() => new Dictionary<ReagentDataSO, float>(_contents);

    [Header("UI Integration")]
    public GameObject labelPrefab;
    public Vector3 labelOffset = new Vector3(0, 0.2f, 0);
    public string DisplayName;

    private GameObject _currentLabelInstance;
    private TextMeshProUGUI _labelText;
    private bool _isHighlighted = false;
    private void Awake()
    {
        _contents.Clear();
        if (initEntry.volume > 0)
        {
            AddLiquidRaw(initEntry.reagent, initEntry.volume);
        }

        if (labelPrefab != null)
        {
            Debug.Log(name + ": UI Initialized");

            _currentLabelInstance = Instantiate(labelPrefab, transform);
            _currentLabelInstance.transform.localPosition = labelOffset;

            _labelText = _currentLabelInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (_labelText != null) _labelText.text = DisplayName;

            _currentLabelInstance.SetActive(false);
        }

        // TODO: 描边特效
        SetHighlight(false);

        // 通知 UI 更新
        RecalculateState();
    }

    private void Update()
    {

        if (_currentLabelInstance == null) return;

        CanvasGroup cg = _currentLabelInstance.GetComponent<CanvasGroup>();
        float targetAlpha = _isHighlighted ? 1.0f : 0.0f;

        // 平滑过渡透明度
        cg.alpha = Mathf.Lerp(cg.alpha, targetAlpha, Time.deltaTime * 10f);

        if (cg.alpha < 0.01f && _currentLabelInstance.activeSelf)
            _currentLabelInstance.SetActive(false);
        else if (cg.alpha > 0.01f && !_currentLabelInstance.activeSelf)
            _currentLabelInstance.SetActive(true);
    }

    public void SetHighlight(bool active)
    {
        _isHighlighted = active;
        // if (_currentLabelInstance != null)
        // {
        //     _currentLabelInstance.SetActive(active);
        // }
    }

    /// <summary>
    /// 添加混合液体
    /// </summary>
    /// <param name="incomingBatch"></param>
    public void AddBatch(Dictionary<ReagentDataSO, float> incomingBatch)
    {
        if (_type == ContainerType.TipBox || _type == ContainerType.Trash) return;

        float incomingVol = incomingBatch.Values.Sum();
        if (incomingVol <= 0) return;

        // 溢出检查
        if (CurrentVolume + incomingVol > _maxVolume)
        {
            Debug.LogWarning($"Container {_containerID} overflow.");
            return;
        }

        // 合并字典
        foreach (var kvp in incomingBatch)
        {
            AddLiquidRaw(kvp.Key, kvp.Value);
        }

        RecalculateState();
    }

    public Dictionary<ReagentDataSO, float> RemoveBatch(float requestedVol)
    {
        var result = new Dictionary<ReagentDataSO, float>();
        if (CurrentVolume <= 0.001f) return result;

        // 计算实际取出量
        float actualVol = Mathf.Min(requestedVol, CurrentVolume);
        float ratio = actualVol / CurrentVolume;

        // 缓存 Keys
        var keys = _contents.Keys.ToList();

        foreach (var key in keys)
        {
            float amountToRemove = _contents[key] * ratio;
            result[key] = amountToRemove;
            _contents[key] -= amountToRemove;

            // 修正浮点数微小误差
            if (_contents[key] < 0.0001f) _contents[key] = 0f;
        }

        RecalculateState();
        return result;
    }

    private void RecalculateState()
    {
        CurrentVolume = 0;
        float r = 0, g = 0, b = 0, a = 0;

        foreach (var kvp in _contents)
        {
            CurrentVolume += kvp.Value;
            // 颜色加权混合
            Color c = kvp.Key.liquidColor;
            r += c.r * kvp.Value;
            g += c.g * kvp.Value;
            b += c.b * kvp.Value;
            a += c.a * kvp.Value;
        }

        if (CurrentVolume > 0)
            CurrentColor = new Color(r / CurrentVolume, g / CurrentVolume, b / CurrentVolume, a / CurrentVolume);
        else
            CurrentColor = Color.clear;
        Debug.Log($"Container: ID = {_containerID}, CurrentVolume = {CurrentVolume}, Color = {CurrentColor}");
        PCREventBus.PublishLiquidUpdate(_containerID, CurrentVolume, CurrentColor);
    }

    private void AddLiquidRaw(ReagentDataSO r, float v)
    {
        if (_contents.ContainsKey(r)) _contents[r] += v;
        else _contents[r] = v;
    }

    public void ReceivePCRResult(bool isSuccess)
    {

    }
}
