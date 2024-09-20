

public interface IDamageReceivable
{
    public Health HP { get; }
    public void OnDeath();
    public void OnDamageRecieve();
}