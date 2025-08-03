using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;
using UnityEngine.TestTools;

public class NewProductConveyComponent : SmithingToolComponent
{
    [SerializeField]
    NewProductConvey convey;
    [SerializeField]
    SmithingToolData data;
    [SerializeField]
    MeshRenderer model;
    [SerializeField]
    Transform productPosition;

    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;


    protected override SmithingTool tool => this.convey;
    protected override ISmithingToolEffecter effecter => null;

    protected override Transform materialPoint => this.productPosition;

    protected override void Update()
    {
    }

    void BeforeInteract(SmithingTool tool)
    {
        if (tool != this.convey)
        {
            return;
        }
    }

    void AfterInteract(SmithingTool tool)
    {
        if (tool != this.convey)
        {
            return;
        }
    }

    protected override void Awake()
    {
        this.meshRenderer = model;
        base.Awake();
        this.convey = new NewProductConvey(this.data);
        this.convey.BeforeInteract += this.BeforeInteract;
        this.convey.AfterInteract += this.AfterInteract;
        //this.uiCanvas.enabled = false;
    }

    public override bool CanWork()
    {
        return false;
    }
    public override ToolWorkResult Work()
    {
        return new ToolWorkResult();
    }
}
