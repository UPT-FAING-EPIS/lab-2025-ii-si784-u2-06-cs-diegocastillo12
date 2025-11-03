using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace UPTSiteTests;

[TestClass]
public class UPTSiteTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await Context.Tracing.StartAsync(new()
        {
            Title = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = Path.Combine(
                Environment.CurrentDirectory,
                "playwright-traces",
                $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}.zip"
            )
        });
        // await Context.CloseAsync();
    }

    [TestMethod]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://www.upt.edu.pe");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Universidad"));
    }

    [TestMethod]
    public async Task GetSchoolDirectorName()
    {
        // Arrange
        string schoolDirectorName = "Ing. Martha Judith Paredes Vignola";
        await Page.GotoAsync("https://www.upt.edu.pe");

        // Act
        await Page.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Pre-Grado" }).HoverAsync(); //ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Escuela Profesional de Ingeniería de Sistemas" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Escuela Profesional de" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Plana Docente" }).ClickAsync();

        // Assert
        await Expect(Page.GetByText("Ing. Martha Judith Paredes")).ToContainTextAsync(schoolDirectorName);
    }

    [TestMethod]
    public async Task SearchStudentInDirectoryPage()
    {
        // Arrange
        string studentName = "AYMA CHOQUE, ERICK YOEL";
        string studentSearch = studentName.Split(" ")[0];
        await Page.GotoAsync("https://www.upt.edu.pe");

        // Act
        await Page.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Pre-Grado" }).HoverAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Escuela Profesional de Ingeniería de Sistemas" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Estudiantes" }).ClickAsync();
    var frameLocator = Page.FrameLocator("iframe");
    await frameLocator.GetByRole(AriaRole.Textbox).ClickAsync();
    await frameLocator.GetByRole(AriaRole.Textbox).FillAsync(studentSearch);
    await frameLocator.GetByRole(AriaRole.Button, new() { Name = "Buscar" }).ClickAsync();
    await frameLocator.GetByRole(AriaRole.Link, new() { Name = "CICLO - VII", Exact = true }).ClickAsync();

    // Assert: buscar en la tabla dentro del iframe
    await Expect(frameLocator.GetByRole(AriaRole.Table)).ToContainTextAsync(studentName);
    }

    // --- New test scenarios added as requested ---
    [TestMethod]
    public async Task CheckAdmissionsPageContainsAdmissionsText()
    {
        // Scenario: open Admissions/Pre-Grado and verify some known heading exists
        await Page.GotoAsync("https://www.upt.edu.pe");
        await Page.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Pre-Grado" }).ClickAsync();

    // A lightweight assertion: page should contain "Pre-Grado" link or text
    await Expect(Page).ToHaveURLAsync(new Regex("upt.edu.pe.*"));
    await Expect(Page.GetByText("Pre-Grado")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CheckFooterContainsContact()
    {
        // Scenario: verify footer contains "Contacto" or similar contact info
        await Page.GotoAsync("https://www.upt.edu.pe");
        await Page.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();

    // Scroll to footer and check for contact-related text
    await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
    // GetByText may match multiple elements (link + heading). Use the first match to avoid strict-mode errors.
    var contactLocator = Page.GetByText(new Regex("Contacto|Contáctenos|Teléfono|E-mail")).First;
    await Expect(contactLocator).ToBeVisibleAsync();
    }
}
