public class PlayerInfo
{
    const int ATTACK_CHANCE = 1;
    public bool isLive{get; set;} = true;
    public bool canShoot{get; set;} = false;
    public int coins{get; set;} = 0;
    public int attackChance{get; set;} = ATTACK_CHANCE;
    public Choice choice{get; set;} = Choice.none;
    public int challengeAmount{get;set;} = 0;

    public int actorNumber{get; private set;}
    
    public PlayerInfo()
    {
        actorNumber = -1;
    }

    public PlayerInfo(int aNum)
    {
        actorNumber = aNum;
    }
}
