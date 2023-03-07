namespace BeastMasterPupExtras;

public partial class BeastMasterPupExtras
{
    public static SlugcatStats.Name StringToSlugName(string data)
    {
        return data switch
        {
            "Yellow" => SlugcatStats.Name.Yellow,
            "White" => SlugcatStats.Name.White,
            "Red" => SlugcatStats.Name.Red,
            "Artificer" => MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Artificer,
            "Gourmand" => MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Gourmand,
            "Spear" => MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear,
            "Rivulet" => MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Rivulet,
            "Saint" => MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint,
            "Sofanthiel" => MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel,
            "Slugpup" => MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Slugpup,
            _ => SlugcatStats.Name.White
        };
    }
    
}