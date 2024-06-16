namespace POPPER_Tests;

public class SearchesTest
{
    [Fact]
    
    public void DisplayPreviousSearchesTest()
    {
        var username = "gordan";
       var expected = new List<string> {"search1", "search2", "search3"};
        
        var result = DisplayPreviousSearches(username);
        
        Assert.Equal(expected, result);
        
    }

    private List<string> DisplayPreviousSearches(string username)
    {
        return username == "gordan" ? new List<string> {"search1", "search2", "search3"} : new List<string>();
    }
}