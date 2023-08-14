using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlaywrightTests;

[TestClass]
public class UnitTest : PageTest
{
    [TestMethod]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    {
        await Page.GotoAsync("https://playwright.dev");

        await Page.SetViewportSizeAsync(1920, 1080);

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

        // create a locator
        var getStarted = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        // Click the get started link.
        await getStarted.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
    }

    [TestMethod]
    public async Task TestWithCustomContextOptions()
    {
        // The following Page (and BrowserContext) instance has the custom colorScheme, viewport and baseURL set:
        await Page.GotoAsync("/login");
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            ColorScheme = ColorScheme.Dark,
            ViewportSize = new()
            {
                Width = 1920,
                Height = 1080
            },
            BaseURL = "https://github.com",
        };
    }

    [TestMethod]
    public async Task HomepageHasOpenTechAllianceInTitleAndLoginLinkLinkingToLoginPage()
    {
        await Page.GotoAsync("https://opentechalliance.com/");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("OpenTech Alliance, Inc."));

        // Create a locator
        var login = Page.GetByRole(AriaRole.Link, new() { Name = "Login" });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(login).ToHaveAttributeAsync("href", "https://opentechalliance.com/customer-login-page/");

        // Click the login link.
        await login.ClickAsync();

        // Expects the URL to contain customer login page.
        await Expect(Page).ToHaveURLAsync(new Regex(".*customer-login-page/"));
    }

    [TestMethod]
    public async Task DevPortalCanBeAccessedThroughHomepage()
    {
        // Go to OpenTech Alliance homepage.
        await Page.GotoAsync("https://opentechalliance.com/");

        // Create a locator for login.
        var login = Page.GetByRole(AriaRole.Link, new() { Name = "Login" });

        // Expect an attribute "to be strictly equal" to the value in the login locator.
        await Expect(login).ToHaveAttributeAsync("href", new Regex("/customer-login-page/"));

        // Click the login link.
        await login.ClickAsync();

        // Get popup after a specific action (e.g., click)
        var popup = await Page.RunAndWaitForPopupAsync(async () =>
        {
            // Create a locator for the Developer API Portal, it should be the 4th Login link on the page
            var devPortal = Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).Nth(3);

            // Click the devPortal link.
            await devPortal.ClickAsync();
        });
        // Wait for popup to load
        await popup.WaitForLoadStateAsync();

        // Expects the popup to contain OpenTech Alliance API Reference in the title
        await Expect(popup).ToHaveTitleAsync(new Regex("OpenTech Alliance API Reference"));

    }
}