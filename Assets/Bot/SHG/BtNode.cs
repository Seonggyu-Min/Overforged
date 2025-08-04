using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public abstract class BtNode 
  {
    public enum NodeState
    {
      Running,      
      Success,
      Failure,
    }

    [SerializeField]
    protected NodeState State;    

    [SerializeField]
    public BtNode Parent;
    [SerializeField]
    protected List<BtNode> children;
    protected Dictionary<string, object> data;
    protected int currentChildIndex;

    public BtNode(BtNode parent = null, IList<BtNode> children = null)
    {
      this.Parent = parent;
      if (children != null) {
        this.children = new List<BtNode>(children.Count);
        foreach (BtNode child in children) {
          this.AddChild(child);
        }
      }
      else {
        this.children = new ();
      }
      this.data = new ();
      this.currentChildIndex = 0;
    }

    public virtual NodeState Evaluate() 
    {
      return (this.children[this.currentChildIndex].Evaluate());
    }

    public virtual void Reset()
    {
      this.currentChildIndex = 0;
      foreach (var child in this.children) {
        child.Reset(); 
      }
    }

    public void SetData(in string key, in object value)
    {
      this.data[key] = value;
    }

    public bool TryFindData(in string key, out object value)
    {
      if (this.data.TryGetValue(key, out value)) {
        return (true);
      }
      if (this.Parent != null) {
        return (this.Parent.TryFindData(key, out value));
      }
      value = null;
      return (false);
    }

    public bool ClearData(in string key)
    {
      if (!this.data.Remove(key)) {
        return (false);
      }
      if (this.Parent != null) {
        return (this.Parent.ClearData(key));
      }
      return (true);
    }

    protected void AddChild(BtNode child)
    {
      child.Parent = this;
      this.children.Add(child);
    }
    
    public NodeState ReturnState(NodeState state)
    {
      this.State = state;
      return (state);
    }
  }
}
