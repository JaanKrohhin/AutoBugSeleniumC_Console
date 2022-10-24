using AutoBugSelenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

Console.WriteLine("Hello testers");
LogHelpers.CheckDirectoryExists();
Console.WriteLine("How many test to launch?");
string filepath = LogHelpers.CreateFolderForSession();
int numberOfTests = GetNumberFromUser("Type in a number between 0 and 13");
List<string> emails = GetEmails(numberOfTests);
Console.WriteLine("Your emails:");
emails.ForEach(Console.WriteLine);
Console.WriteLine("\nPress ENTER to start the program. Once started, press ENTER again to stop finish testing.\nThe program will close after it finishes the current tests");
Console.ReadLine();
int i = 1;
foreach (var item in emails)
{
    try
    {
        new Thread(() => TestSite(item, i++, filepath)).Start();
        new Thread(() => Comparison(i++)).Start();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    
}
Console.ReadLine();
LogHelpers.EndAllTests = false;
#region tests
void TestSite(string email, int testNumber,string filepath)
{
    LogHelpers log = new LogHelpers(filepath);
    log.CreateLogFile(testNumber);
    log.WriteToFile("TestEmail: "+email);
    do
    {
        try
        {
            using (IWebDriver driver = DriverSetup())
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Login(driver, email);
                Thread.Sleep(3000);
                PutItemToCartAndGoToCart(wait, driver);
                Thread.Sleep(3000);
                AccountInfo(wait, driver);
                Thread.Sleep(3000);
                ConfrimDelivery(wait, driver);
                log.WriteToFile("Passed");
            }
        }
        catch (Exception e)
        {
            log.WriteToFile("Error "+e.ToString());
            log.WriteToFile("Restarting");
        }
    } while (LogHelpers.EndAllTests);
}

 void Comparison(int testNumber)
{
    do
    {
        using (IWebDriver dr = DriverSetup())
        {
            LogHelpers log = new LogHelpers(filepath);
            log.CreateLogFile(testNumber);
            log.Info("Start comparison testing, test number: " + testNumber);
            List<string> items = new List<string>();
            List<IWebElement> buttons = new List<IWebElement>();
            List<string> comparing = new List<string>();
            dr.Manage().Window.Maximize();
            dr.Navigate().GoToUrl("https://hgdft53.frog.ee/en/1615/pc-components");
            var products = dr.FindElements(By.ClassName("ajax_block_product"));
            int count = 0;
            foreach (var item in products)
            {
                if (count == 4)
                    break;
                buttons.Add(item.FindElements(By.ClassName("add_to_compare"))[0]);
                items.Add(item.FindElement(By.ClassName("product-name")).GetAttribute("title"));
                count++;
            }
            int pressed = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                if (pressed == 4)
                    break;
                if (buttons[i].Displayed)
                {
                    WebDriverWait wait = new WebDriverWait(dr, TimeSpan.FromSeconds(10));
                    buttons[i].Click();
                    pressed++;
                    wait.Until((d) =>
                    {
                        if (dr.FindElements(By.CssSelector("a.add_to_compare.checked")).Count == pressed)
                        {
                            return dr;
                        }
                        return null;
                    });

                }
                dr.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            }
            new WebDriverWait(dr, TimeSpan.FromSeconds(10));
            dr.FindElement(By.ClassName("bt_compare")).Click();
            foreach (var item in dr.FindElements(By.ClassName("ajax_block_product")))
            {
                try
                {
                    string itemName = item.FindElement(By.ClassName("product-name")).GetAttribute("title");
                    if (items.Contains(itemName))
                    {
                        items.Remove(itemName);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }
            }
            if (pressed != 0)
                log.Info("Successfully added to comparison");
            //deleting
            while (pressed != 0)
            {
                var item = dr.FindElement(By.ClassName("ajax_block_product"));
                try
                {
                    IWebElement deleteButton = item.FindElement(By.ClassName("cmp_remove"));
                    WebDriverWait wait = new WebDriverWait(dr, TimeSpan.FromSeconds(60));
                    if (deleteButton.Displayed)
                    {
                        deleteButton.Click();
                        pressed--;
                        wait.Until((d) =>
                        {
                            if (dr.FindElements(By.ClassName("cmp_remove")).Count == pressed)
                            {
                                return dr;
                            }
                            return null;
                        });
                    }
                }
                catch (StaleElementReferenceException ex)
                {
                    item = dr.FindElement(By.ClassName("ajax_block_product"));
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }
            }
            if (pressed == 0)
                log.Info("Successfully deleted from comparison");
            log.Info("End comparison testing " + testNumber);
        }
    } while (LogHelpers.EndAllTests);
}
#endregion
IWebDriver DriverSetup()
{
    IWebDriver driver = new EdgeDriver();
    driver.Manage().Window.Maximize();
    driver.Url = "https://hgdft53.frog.ee";
    return driver;
}

void Login(IWebDriver driver, string email)
{
    driver.FindElement(By.XPath("/html/body/div[2]/div[1]/header/div[2]/div/div[8]/div")).Click();
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
    driver.FindElement(By.CssSelector("#header-email")).SendKeys(email);
    driver.FindElement(By.CssSelector("#header-passwd")).SendKeys("12345");
    driver.FindElement(By.CssSelector("#header-login > ul > li.login-content > form > div.form_content.clearfix > p.submit > button")).Click();
}

void PutItemToCartAndGoToCart(WebDriverWait wait, IWebDriver driver)
{
    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#homecategoryproducts_wrap_1354 > ul:nth-child(3) > div:nth-child(2) > div:nth-child(1) > li:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(4) > ul:nth-child(2) > li:nth-child(1) > a:nth-child(1)")));
    driver.FindElement(By.CssSelector("#homecategoryproducts_wrap_1354 > ul:nth-child(3) > div:nth-child(2) > div:nth-child(1) > li:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(4) > ul:nth-child(2) > li:nth-child(1) > a:nth-child(1)")).Click();

    driver.FindElement(By.CssSelector("#header > div.container.container-fluid.mainf > div > div.clearfix.shopping_cart_wrap > div > a")).Click();
    Thread.Sleep(500);

}

void AccountInfo(WebDriverWait wait, IWebDriver driver)
{
    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector("#id_state_delivery")));
    new SelectElement(driver.FindElement(By.CssSelector("#id_state_delivery"))).SelectByText(".Tallinn");

    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#contact_phone")));
    var phone = driver.FindElement(By.Id("contact_phone"));
    phone.Click();
    phone.Clear();
    phone.SendKeys("+3728888888788888");
    phone.Submit();

}

void ConfrimDelivery(WebDriverWait wait, IWebDriver driver)
{
    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#submitAddressInvoice")));
    driver.FindElement(By.CssSelector("#submitAddressInvoice")).Click();
    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#submitConfirmDelivery")));
    driver.FindElement(By.CssSelector("#submitConfirmDelivery")).Click();
    driver.Navigate().GoToUrl("https://hgdft53.frog.ee/et/mod/bankwire/payment");

    wait.Timeout = TimeSpan.FromSeconds(5);

    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector("#cart_navigation > button")));
    driver.FindElement(By.CssSelector("#cart_navigation > button")).Click();
    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector("#order-confirmation-page")));
}

int GetNumberFromUser(string message, int min = 0, int max = 12)
{
    int a = 0; 
    while (a <= min || a > max)
    {
        try
        {
            Console.WriteLine(message);
            a = Convert.ToInt32(Console.ReadLine());
        }
        catch (Exception)
        {
            Console.WriteLine("Not a number");
        }
        Console.WriteLine("\n");
    }
    return a;
}
List<string> GetEmails(int maxEmails)
{
    List<string> emails = new List<string>();
    int startingEmailnumber = GetNumberFromUser("Type in a number for the starting email(1-96)", max: 96);
    for (int i = startingEmailnumber; i < startingEmailnumber+maxEmails; i++)
    {
        emails.Add("testReg" + i + "@superFakeMail.TestsJK");
    }
    return emails;
}
