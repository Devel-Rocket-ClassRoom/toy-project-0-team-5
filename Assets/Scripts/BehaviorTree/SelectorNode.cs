public class SelectorNode : CompositeNode
{
    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            var result = child.Evaluate();
            if (result != NodeState.Failure) return result;
        }
        return NodeState.Failure;
    }
}
