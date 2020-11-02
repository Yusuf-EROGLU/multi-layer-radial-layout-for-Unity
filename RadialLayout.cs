using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class RadialLayout : LayoutGroup
{
    public float fDistance;
    public float layerDistance;
    public int layerNum;

    private List<int> _layersCapasity;

    [Range(0f, 360f)] public float MinAngle, MaxAngle, StartAngle;

    protected override void OnEnable()
    {
        base.OnEnable();
        CalculateRadial();
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }

    public override void CalculateLayoutInputVertical()
    {
        CalculateRadial();
    }

    public override void CalculateLayoutInputHorizontal()
    {
        CalculateRadial();
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        CalculateRadial();
    }
#endif
    void CalculateRadial()
    {
        m_Tracker.Clear();
        if (transform.childCount == 0)
            return;
        CalculateLayerSize();

        int childIndex = 0;
        for (int z = 0; z < layerNum; z++)
        {
            float fOffsetAngle = ((MaxAngle - MinAngle)) / (_layersCapasity[z] );
            float fAngle = StartAngle;

            for (int i = 0; i < _layersCapasity[z]; i++)
            {
                RectTransform child = (RectTransform) transform.GetChild(childIndex);
                if (child != null)
                {
                    //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                    m_Tracker.Add(this, child,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.Pivot);

                    Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);

                    child.localPosition = vPos * (fDistance + z * layerDistance);

                    //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                    child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
                    fAngle += fOffsetAngle;
                }

                childIndex++;
            }
        }
    }

    private void CalculateLayerSize()
    {
        List<float> peripheralLengths = new List<float>();
        float sumOfPhLengths = 0;
        _layersCapasity = new List<int>();

        for (int i = 0; i < layerNum; i++)
        {
            peripheralLengths.Add(2 * Mathf.PI * (fDistance + layerDistance * i));
        }

        peripheralLengths.ForEach(x => sumOfPhLengths += x);

        foreach (var length in peripheralLengths)
        {
            _layersCapasity.Add((int) (transform.childCount*length / sumOfPhLengths));
        }

        if (_layersCapasity.Sum() < transform.childCount)
        {
            _layersCapasity[layerNum - 1] += transform.childCount-_layersCapasity.Sum();
        }
    }
}