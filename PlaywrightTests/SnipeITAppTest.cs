using Microsoft.Playwright;

namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class SnipeITAppTest : PageTest
    {
        [Test]
        public async Task UserCanCreateAssetAndCheckoutToUser()
        {
            // Login to the snipeit demo            
            await Page.GotoAsync("https://demo.snipeitapp.com/login");

            var usernameTextbox = Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" });
            await usernameTextbox.FillAsync("admin");

            var passwordTextbox = Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
            await passwordTextbox.FillAsync("password");

            var loginButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login" });
            await loginButton.ClickAsync();

            await Expect(Page).ToHaveTitleAsync(new Regex("Dashboard"));

            // Navigate to Create Asset page
            var createNewDropdown = Page.GetByText("Create New");
            await createNewDropdown.ClickAsync();
            var assetOption = Page.GetByRole(AriaRole.Navigation).GetByText("Asset", new() { Exact = true });
            await assetOption.ClickAsync();

            await Expect(Page).ToHaveTitleAsync(new Regex("Create Asset :: Snipe-IT Asset Management Demo"));

            // Create a new Macbook Pro13" asset
            var companyCombobox = Page.GetByRole(AriaRole.Combobox, new() { Name = "Select Company", Exact = true });
            await companyCombobox.ClickAsync();
            var companyOption = Page.GetByRole(AriaRole.Option);
            await companyOption.Nth(2).ClickAsync();

            var assetTextBox = Page.GetByRole(AriaRole.Textbox, new() { Name = "Asset Tag", Exact = true });
            string assetTag = $"Test-{Guid.NewGuid()}";
            await assetTextBox.FillAsync(assetTag);

            var modelCombobox = Page.GetByRole(AriaRole.Combobox, new() { Name = "Select a Model", Exact = true });
            await modelCombobox.ClickAsync();
            var modelOption = Page.GetByRole(AriaRole.Option).GetByText("Laptops - Macbook Pro 13\"");
            await modelOption.ClickAsync();

            // Set the ready to deploy status
            var statusCombobox = Page.GetByRole(AriaRole.Combobox, new() { Name = "Select Status", Exact = true });
            await statusCombobox.ClickAsync();
            var statusOption = Page.GetByRole(AriaRole.Option).GetByText("Ready to Deploy");
            await statusOption.ClickAsync();

            // Checkout asset to random user
            var userCombobox = Page.GetByRole(AriaRole.Combobox, new() { Name = "Select a User", Exact = true });
            await userCombobox.ClickAsync();
            var userOption = Page.GetByRole(AriaRole.Option).Nth(2);
            await userOption.ClickAsync();

            var locationCombobox = Page.GetByRole(AriaRole.Combobox, new() { Name = "Select a Location", Exact = true });
            await locationCombobox.ClickAsync();
            var locationOption = Page.GetByRole(AriaRole.Option).Nth(2);
            await locationOption.ClickAsync();

            var submitButton = Page.Locator("#submit_button");
            await submitButton.ClickAsync();

            await Expect(Page).ToHaveTitleAsync(new Regex("Dashboard"));

            // Find the asset in the assets list to verify it was created successfully
            await Page.GotoAsync("https://demo.snipeitapp.com/reports/activity");

            // Find the asset using the AssetTag
            var searchTextbox = Page.GetByRole(AriaRole.Textbox, new() { Name = "Search" });
            await searchTextbox.FillAsync(assetTag);

            // Open the asset
            var activityTable = Page.Locator("#activityReport");
            await Expect(activityTable).ToBeVisibleAsync();

            var activityTableHeaders = await activityTable.Locator("th").AllAsync();
            if (activityTableHeaders.Count != 10)
            {
                throw new Exception("activity table does not have 10 columns");
            }

            var activityTableRows = await activityTable.Locator("tr").AllAsync();
            if (activityTableRows.Count != 2)
            {
                throw new Exception("There should only be two rows in the table");
            }
        }
    }
}
