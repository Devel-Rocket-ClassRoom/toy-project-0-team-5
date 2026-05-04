using System.Collections.Generic;

public abstract class CompositeNode : BehaviorNode
{
    protected readonly List<BehaviorNode> children = new();

    public CompositeNode AddChild(BehaviorNode child)
    {
        children.Add(child);
        return this;
    }
}
