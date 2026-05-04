public class SequenceNode : CompositeNode
{
    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            var result = child.Evaluate();
            if (result != NodeState.Success) return result;
        }
        return NodeState.Success;
    }
}
