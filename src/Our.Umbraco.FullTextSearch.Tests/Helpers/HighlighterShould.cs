using Our.Umbraco.FullTextSearch.Helpers;

namespace Our.Umbraco.FullTextSearch.Tests.Helpers;

public class HighlighterShould
{
    [Test]
    public void Highlight_Exact_Matches()
    {
        var test = Highlighter.FindSnippet("This token should be highlighted, but not al tokens will be. Just that token in the beginning.", "token", 200, "<b>{0}</b>");

        Assert.That(test, Is.EqualTo("This <b>token</b> should be highlighted, but not al tokens will be. Just that <b>token</b> in the beginning"));

    }

    [Test]
    public void FindSnippet_And_Truncate()
    {
        var test = Highlighter.FindSnippet("This token should be highlighted, but not al tokens will be. Just that token in the beginning.", "token", 30, "<b>{0}</b>");

        Assert.That(test, Is.EqualTo(" Just that <b>token</b> in the beginn"));

    }
}