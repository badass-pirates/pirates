public class PlayerInfo
{
    public bool isLive{get; set;}
    public bool canShoot{get; set;}
    public int coins{get; set;}
    public int attackChance{get; set;}
    public Choice choice{get; set;}
    public int challengeAmount{get;set;}
    
    public PlayerInfo()
    {
        isLive = true;
        canShoot = false;
        coins = 0;
        attackChance = 1;
        choice = Choice.ready;
        challengeAmount = 0;
    }
}
