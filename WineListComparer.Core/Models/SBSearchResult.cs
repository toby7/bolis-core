
using System.Text.Json.Serialization;

public class SbSearchResult
{
    public string SearchSentence { get; set; }
    [JsonPropertyName("products")]
    public Product[]? Products { get; set; }
    //public Metadata metadata { get; set; }
    //public Filter[] filters { get; set; }
    //public object[] filterMenuItems { get; set; }
}

public class Metadata
{
    public int docCount { get; set; }
    public int fullAssortmentDocCount { get; set; }
    public int nextPage { get; set; }
    public Pricerange priceRange { get; set; }
    public Volumerange volumeRange { get; set; }
    public object didYouMeanQuery { get; set; }
}

public class Pricerange
{
    public float min { get; set; }
    public float max { get; set; }
}

public class Volumerange
{
    public float min { get; set; }
    public float max { get; set; }
}


public class Product
{
    public string productId { get; set; }
    public string productNumber { get; set; }
    public string productNameBold { get; set; }
    public string productNameThin { get; set; }
    public object category { get; set; }
    public string productNumberShort { get; set; }
    public string producerName { get; set; }
    public string supplierName { get; set; }
    public bool isKosher { get; set; }
    public string bottleTextShort { get; set; }
    public int restrictedParcelQuantity { get; set; }
    public bool isOrganic { get; set; }
    public bool isEthical { get; set; }
    public object ethicalLabel { get; set; }
    public bool isWebLaunch { get; set; }
    public DateTime productLaunchDate { get; set; }
    public bool isCompletelyOutOfStock { get; set; }
    public bool isTemporaryOutOfStock { get; set; }
    public float alcoholPercentage { get; set; }
    public string volumeText { get; set; }
    public float volume { get; set; }
    public float price { get; set; }
    public string country { get; set; }
    public string originLevel1 { get; set; }
    public string originLevel2 { get; set; }
    public string categoryLevel1 { get; set; }
    public string categoryLevel2 { get; set; }
    public object categoryLevel3 { get; set; }
    public object categoryLevel4 { get; set; }
    public string customCategoryTitle { get; set; }
    public string assortmentText { get; set; }
    public object usage { get; set; }
    public object taste { get; set; }
    public object[] tasteSymbols { get; set; }
    public object tasteClockGroupBitter { get; set; }
    public object tasteClockGroupSmokiness { get; set; }
    public int tasteClockBitter { get; set; }
    public int tasteClockFruitacid { get; set; }
    public int tasteClockBody { get; set; }
    public int tasteClockRoughness { get; set; }
    public int tasteClockSweetness { get; set; }
    public int tasteClockSmokiness { get; set; }
    public int tasteClockCasque { get; set; }
    public string assortment { get; set; }
    public float recycleFee { get; set; }
    public bool isManufacturingCountry { get; set; }
    public bool isRegionalRestricted { get; set; }
    public string packaging { get; set; }
    public bool isNews { get; set; }
    public Image[] images { get; set; }
    public bool isDiscontinued { get; set; }
    public bool isSupplierTemporaryNotAvailable { get; set; }
    public int sugarContent { get; set; }
    public float sugarContentGramPer100ml { get; set; }
    public object[] seal { get; set; }
    public string vintage { get; set; }
    public object[] grapes { get; set; }
    public object otherSelections { get; set; }
    public object[] tasteClocks { get; set; }
    public object color { get; set; }
    public object dishPoints { get; set; }
}

public class Image
{
    public string imageUrl { get; set; }
    public object fileType { get; set; }
    public object size { get; set; }
}

public class Filter
{
    public string name { get; set; }
    public string type { get; set; }
    public string displayName { get; set; }
    public string description { get; set; }
    public bool isMultipleChoice { get; set; }
    public bool isActive { get; set; }
    public Searchmodifier3[] searchModifiers { get; set; }
    public Child child { get; set; }
}

public class Child
{
    public string name { get; set; }
    public string type { get; set; }
    public string displayName { get; set; }
    public string description { get; set; }
    public bool isMultipleChoice { get; set; }
    public bool isActive { get; set; }
    public Searchmodifier2[] searchModifiers { get; set; }
    public Child1 child { get; set; }
}

public class Child1
{
    public string name { get; set; }
    public string type { get; set; }
    public string displayName { get; set; }
    public object description { get; set; }
    public bool isMultipleChoice { get; set; }
    public bool isActive { get; set; }
    public Searchmodifier1[] searchModifiers { get; set; }
    public Child2 child { get; set; }
}

public class Child2
{
    public string name { get; set; }
    public string type { get; set; }
    public string displayName { get; set; }
    public object description { get; set; }
    public bool isMultipleChoice { get; set; }
    public bool isActive { get; set; }
    public Searchmodifier[] searchModifiers { get; set; }
    public object child { get; set; }
}

public class Searchmodifier
{
    public string value { get; set; }
    public int count { get; set; }
    public bool isActive { get; set; }
    public object friendlyUrl { get; set; }
}

public class Searchmodifier1
{
    public string value { get; set; }
    public int count { get; set; }
    public bool isActive { get; set; }
    public object friendlyUrl { get; set; }
}

public class Searchmodifier2
{
    public string value { get; set; }
    public int count { get; set; }
    public bool isActive { get; set; }
    public object friendlyUrl { get; set; }
}

public class Searchmodifier3
{
    public string value { get; set; }
    public int count { get; set; }
    public bool isActive { get; set; }
    public object friendlyUrl { get; set; }
}
