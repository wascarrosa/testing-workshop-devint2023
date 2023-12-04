

using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

var wiremockServer = WireMockServer.Start();

Console.WriteLine($"WireMock.NET is now running on: {wiremockServer.Url}");

wiremockServer.Given(
    Request.Create().WithPath("/users/nickchapsas").UsingGet())
    .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithHeader("Content-Type", "application/json; charset=utf-8")
        .WithBody(GenerateGitHubUserResponseBody("nickchapsas")));

Console.ReadKey();
wiremockServer.Dispose();

static string GenerateGitHubUserResponseBody(string username)
{
    return $@"{{
  ""login"": ""{username}"",
  ""id"": 67104228,
  ""node_id"": ""MDQ6VXNlcjY3MTA0MjI4"",
  ""avatar_url"": ""https://avatars.githubusercontent.com/u/67104228?v=4"",
  ""gravatar_id"": """",
  ""url"": ""https://api.github.com/users/{username}"",
  ""html_url"": ""https://github.com/{username}"",
  ""followers_url"": ""https://api.github.com/users/{username}/followers"",
  ""following_url"": ""https://api.github.com/users/{username}/following{{/other_user}}"",
  ""gists_url"": ""https://api.github.com/users/{username}/gists{{/gist_id}}"",
  ""starred_url"": ""https://api.github.com/users/{username}/starred{{/owner}}{{/repo}}"",
  ""subscriptions_url"": ""https://api.github.com/users/{username}/subscriptions"",
  ""organizations_url"": ""https://api.github.com/users/{username}/orgs"",
  ""repos_url"": ""https://api.github.com/users/{username}/repos"",
  ""events_url"": ""https://api.github.com/users/{username}/events{{/privacy}}"",
  ""received_events_url"": ""https://api.github.com/users/{username}/received_events"",
  ""type"": ""User"",
  ""site_admin"": false,
  ""name"": null,
  ""company"": null,
  ""blog"": """",
  ""location"": null,
  ""email"": null,
  ""hireable"": null,
  ""bio"": null,
  ""twitter_username"": null,
  ""public_repos"": 0,
  ""public_gists"": 0,
  ""followers"": 0,
  ""following"": 0,
  ""created_at"": ""2020-06-18T11:47:58Z"",
  ""updated_at"": ""2020-06-18T11:47:58Z""
}}";
}
