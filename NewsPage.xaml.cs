using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using CodeHollow.FeedReader;
using HtmlAgilityPack;
using Xamarin.Essentials;

namespace gorsel_programlam_odev2;

public partial class NewsPage : ContentPage
{
    private const string RssUrl = "https://www.trthaber.com/spor_articles.rss";
    private const string SondakikaRssUrl = "https://www.trthaber.com/sondakika_articles.rss";
    private const string GundemRssUrl = "https://www.trthaber.com/gundem_articles.rss";
    private const string EkonomiRssUrl = "https://www.trthaber.com/ekonomi_articles.rss";
    private const string BilimTeknolojiRssUrl = "https://www.trthaber.com/bilim_teknoloji_articles.rss";

    // Create a single static HttpClient instance for the entire application
    private static readonly HttpClient _httpClient = new HttpClient();

    public NewsPage()
    {
        InitializeComponent();
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        App.ToggleTheme(e.Value);
    }

    private async void OnSonDakikaButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var rssFeed = await _httpClient.GetStringAsync(SondakikaRssUrl);
            var feed = FeedReader.ReadFromString(rssFeed);

            var sonDakikaPage = new ContentPage
            {
                Title = "Son Dakika Haberler",
                Content = await CreateNewsListView(feed.Items)
            };

            sonDakikaPage.ToolbarItems.Clear();

            await Navigation.PushAsync(sonDakikaPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnSporButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var rssFeed = await _httpClient.GetStringAsync(RssUrl);
            var feed = FeedReader.ReadFromString(rssFeed);

            var newsPage = new ContentPage
            {
                Title = "Spor Haberler",
                Content = await CreateNewsListView(feed.Items)
            };

            newsPage.ToolbarItems.Clear();

            await Navigation.PushAsync(newsPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnGundemButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var rssFeed = await _httpClient.GetStringAsync(GundemRssUrl);
            var feed = FeedReader.ReadFromString(rssFeed);

            var gundemPage = new ContentPage
            {
                Title = "Gündem Haberler",
                Content = await CreateNewsListView(feed.Items)
            };

            gundemPage.ToolbarItems.Clear();

            await Navigation.PushAsync(gundemPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnEkonomiButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var rssFeed = await _httpClient.GetStringAsync(EkonomiRssUrl);
            var feed = FeedReader.ReadFromString(rssFeed);

            var ekonomiPage = new ContentPage
            {
                Title = "Ekonomi Haberler",
                Content = await CreateNewsListView(feed.Items)
            };

            ekonomiPage.ToolbarItems.Clear();

            await Navigation.PushAsync(ekonomiPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnBilimTeknolojiButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var rssFeed = await _httpClient.GetStringAsync(BilimTeknolojiRssUrl);
            var feed = FeedReader.ReadFromString(rssFeed);

            var bilimTeknolojiPage = new ContentPage
            {
                Title = "Bilim ve Teknoloji Haberler",
                Content = await CreateNewsListView(feed.Items)
            };

            bilimTeknolojiPage.ToolbarItems.Clear();

            await Navigation.PushAsync(bilimTeknolojiPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnRefreshButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var rssFeed = await _httpClient.GetStringAsync(RssUrl);
            var feed = FeedReader.ReadFromString(rssFeed);

            var listView = await CreateNewsListView(feed.Items);

            await DisplayAlert("basarili", "haberler basariyla yenilendi.", "tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private bool shouldShareAfterNavigation = false;
    private string webViewPageTitle = "";

    private async void OnShareButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var selectedItem = ((View)sender)?.BindingContext as NewsItem;

            if (selectedItem != null)
            {
                var htmlContent = await GetHtmlContent(selectedItem.Link);

                var webViewPage = new ContentPage
                {
                    Title = selectedItem.Title
                };

                var webView = new WebView
                {
                    Source = new HtmlWebViewSource { Html = htmlContent },
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                var shareButton = new ToolbarItem
                {
                    Text = "Share",
                    Command = new Command(() => ShareUri(selectedItem.Link, selectedItem.Title))
                };

                webViewPage.ToolbarItems.Add(shareButton);

                webViewPage.Content = webView;

                webView.Navigated += (s, args) => WebViewNavigated(s, args, selectedItem.Title);

                await Navigation.PushAsync(webViewPage);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void WebViewNavigated(object sender, WebNavigatedEventArgs e, string pageTitle)
    {
        if (shouldShareAfterNavigation)
        {
            shouldShareAfterNavigation = false;
            var webView = sender as WebView;
            if (webView != null)
            {
                webView.Navigated -= (s, args) => WebViewNavigated(s, args, pageTitle);
                await ShareUri(webView.Source.ToString(), pageTitle);
            }
        }
    }

    public async Task ShareUri(string uri, string title)
    {
        await Xamarin.Essentials.Share.RequestAsync(new Xamarin.Essentials.ShareTextRequest
        {
            Uri = uri,
            Title = title
        });
    }

    private async Task<View> CreateNewsListView(IEnumerable<FeedItem> items)
    {
        var newsData = await GetNewsData(items);

        var listView = new ListView
        {
            ItemsSource = newsData,
            ItemTemplate = new DataTemplate(() =>
            {
                var title = new Label();
                title.SetBinding(Label.TextProperty, "Title");

                var summary = new Label();
                summary.SetBinding(Label.TextProperty, "Summary");

                var link = new Label();
                link.SetBinding(Label.TextProperty, "Link");

                var image = new Image();
                image.SetBinding(Image.SourceProperty, "Image");
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (sender, e) =>
                {
                    var selectedItem = ((View)sender)?.BindingContext as NewsItem;

                    if (selectedItem != null)
                    {
                        var htmlContent = await GetHtmlContent(selectedItem.Link);

                        var webViewPage = new ContentPage
                        {
                            Title = selectedItem.Title
                        };

                        var webView = new WebView
                        {
                            Source = new HtmlWebViewSource { Html = htmlContent },
                            VerticalOptions = LayoutOptions.FillAndExpand
                        };

                        var shareButton = new ToolbarItem
                        {
                            Text = "Share",
                            Command = new Command(async () =>
                            {
                                await Xamarin.Essentials.Share.RequestAsync(new Xamarin.Essentials.ShareTextRequest
                                {
                                    Text = $"{selectedItem.Title}\n{selectedItem.Link}",
                                    Title = "Share News"
                                });
                            })
                        };

                        webViewPage.ToolbarItems.Add(shareButton);

                        webViewPage.Content = webView;

                        await Navigation.PushAsync(webViewPage);
                    }
                };
                title.GestureRecognizers.Add(tapGestureRecognizer);
                summary.GestureRecognizers.Add(tapGestureRecognizer);
                link.GestureRecognizers.Add(tapGestureRecognizer);
                image.GestureRecognizers.Add(tapGestureRecognizer);

                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Padding = new Thickness(10),
                        Children =
                        {
                            title,
                            summary,
                            link,
                            image
                        }
                    }
                };
            })
        };

        return listView;
    }

    private async Task<List<NewsItem>> GetNewsData(IEnumerable<FeedItem> items)
    {
        var newsData = new List<NewsItem>();

        foreach (var item in items)
        {
            var htmlContent = await GetHtmlContent(item.Link);
            var (title, text, imageUrl) = ExtractArticleData(htmlContent);

            newsData.Add(new NewsItem
            {
                Title = title,
                Summary = item.Description,
                Link = item.Link,
                Image = imageUrl
            });
        }

        return newsData;
    }

    private async Task<string> GetHtmlContent(string url)
    {
        return await _httpClient.GetStringAsync(url);
    }

    private (string Title, string Text, string Image) ExtractArticleData(string htmlContent)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        var titleNode = htmlDocument.DocumentNode.SelectSingleNode("//h1[@class='page-title']");
        var textNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='description']");
        var imageNode = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'standard-right-thumb-card')]/div[@class='image-frame']/a/picture/source");

        var title = titleNode?.InnerText.Trim();
        var text = textNode?.InnerText.Trim();
        var imageUrl = imageNode?.GetAttributeValue("src", null);

        return (title, text, imageUrl);
    }
}