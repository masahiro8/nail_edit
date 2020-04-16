using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class MultiScroller : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    [SerializeField]
    private IDragHandler[] m_parentDragHandlers;
    private IBeginDragHandler[] m_parentBeginDragHandlers;
    private IEndDragHandler[] m_parentEndDragHandlers;

    private bool isSelf = false;
    void Start () {
        m_parentDragHandlers = this.GetComponentsInParent<IDragHandler>().Where(p => !(p is MultiScroller)).ToArray();
        m_parentBeginDragHandlers = this.GetComponentsInParent<IBeginDragHandler>().Where(p => !(p is MultiScroller)).ToArray();
        m_parentEndDragHandlers = this.GetComponentsInParent<IEndDragHandler>().Where(p => !(p is MultiScroller)).ToArray();
    }

    public void OnDrag(PointerEventData ped)
    {
        foreach(var dr in m_parentDragHandlers)
        {
            dr.OnDrag(ped);
        }
    }
    public void OnBeginDrag(PointerEventData ped)
    {
        foreach(var dr in m_parentBeginDragHandlers)
        {
            dr.OnBeginDrag(ped);
        }
    }
    public void OnEndDrag(PointerEventData ped)
    {
        foreach(var dr in m_parentEndDragHandlers)
        {
            dr.OnEndDrag(ped);
        }
    }
}
