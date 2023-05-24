using System.Collections.Generic;

public class HaulItemJob : Job
{
    private readonly JobDesignation owner;

    public override JobDesignation Owner => owner;

    public RetrieveItemTarget Source { get; private set; }
    public DepositItemTarget Destination { get; private set; }
    public Item Item { get; private set; }

    private readonly string displayName;
    public override string DisplayName => displayName;

    public HaulItemJob(HaulDesignation owner, DepositItemTarget destination, Item item)
    {
        this.owner = owner;
        Source = owner.Source;
        Destination = destination;
        Item = item;
        displayName = $"Haul {Item.name} from {Source.name} to {Destination.name}";
    }

    public override ICommand CreateCommand(ActorAI actor)
    {
        IEnumerable<ICommand> commands = new List<ICommand>
        {
            new ApproachCommand(actor.NavMeshAgent, Source.transform),
            new TransferItemsCommand(Source.Inventory, actor.Inventory, Item),
            new ApproachCommand(actor.NavMeshAgent, Destination.transform),
            new TransferItemsCommand(actor.Inventory, Destination.Inventory, Item)
        };
        return new CompositeCommand(commands);
    }

    public override bool CanPerformWith(ActorAI actor)
    {
        return actor.NavMeshAgent != null && actor.Inventory != null;
    }

    public override bool IsValid()
    {
        bool isValid = true;
        isValid &= Source.gameObject.activeSelf;
        isValid &= Destination.gameObject.activeSelf;
        isValid &= Source.Inventory.Count(Item) > 0;
        return isValid;
    }
}
