public class PlayerInfo
{
    public bool isLive{get; set;}
    public bool isShoot{get; set;}
    public int coins{get; set;}
    public int attackChance{get; set;}
    public Choice choice{get; set;}
    public int challengeAmount{get;set;}
    public bool isSuccess{get; set;}
    
    public PlayerInfo()
    {
        isLive = true;
        isShoot = false;
        coins = 0;
        attackChance = 1;
        choice = Choice.ready;
        challengeAmount = 0;
        isSuccess = false;
    }
}
