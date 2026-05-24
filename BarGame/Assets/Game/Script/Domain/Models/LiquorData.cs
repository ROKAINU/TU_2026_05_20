namespace Game.Domain
{
    // 材料の種類
    public enum IngredientType
    {
        Base,      // ウイスキー、ジンなど
        Liqueur,   // リキュール系
        Mixer,     // 割り材（炭酸、ジュースなど）
    }

    // 素材ID（調合に使う材料）
    public enum IngredientId
    {
        // ベース
        Whiskey,
        Gin,
        Vodka,
        Rum,
        Tequila,

        // リキュール
        Vermouth,
        Grenadine,
        Cassis,
        CoffeeLiqueur,
        Campari,

        // 割り材
        Soda,
        TonicWater,
        OrangeJuice,
        Milk,
        Lime,
    }

    // タグ（特徴やイメージ）
    public enum LiquorTag
    {
        Sweet,//甘い
        Refreshing,//爽やか
        Strong,//強い
        Comforting,//癒し
        Calm,//落ち着き
        Bitter,//苦い
        Light,//軽い
        Fizzy,//刺激的
        // 追加のタグはここに列挙
    }

    // 完成品ID（提供できるもの全て）
    public enum DrinkId
    {
        // ベース単体（そのまま提供）
        Whiskey,
        Gin,
        Vodka,
        Rum,
        Tequila,

        // カクテル
        Highball,
        GinTonic,
        RumTonic,
        VodkaTonic,
        CassisOrange,
        CassisMilk,
        CassisSoda,
        KaluaMilk,
        BlackRussian,
        KaluaSoda,
        ScrewDriver,
        CubaLibre,
        RumSoda,
        VodkaSoda,
        VodkaLime,
        Gimlet,
        GinSoda,
        TequilaSoda,
        TequilaSunrise,
        CampariSoda,
        CampariOrange,
        Negroni,
        Martini,
        WhiskeySoda,
        WhiskeyOrange,
    }
}