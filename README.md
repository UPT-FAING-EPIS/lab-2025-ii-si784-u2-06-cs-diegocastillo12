[![Open in Codespaces](https://classroom.github.com/assets/launch-codespace-2972f46106e565e64193e422d61a12cf1da4916b45550586e14ef0a7c637dd04.svg)](https://classroom.github.com/open-in-codespaces?assignment_repo_id=21448850)
# SESION DE LABORATORIO N° 06: PRUEBAS DE INTERFAZ DE USUARIO

### Nombre:
Diego Fernando Castillo Mamani

## OBJETIVOS
  * Comprender el funcionamiento de las pruebas de interfaz de usuario en una aplicación web.

## REQUERIMIENTOS
  * Conocimientos: 
    - Conocimientos básicos de Bash (powershell).
    - Conocimientos básicos de Contenedores (Docker).
  * Hardware:
    - Virtualization activada en el BIOS..
    - CPU SLAT-capable feature.
    - Al menos 4GB de RAM.
  * Software:
    - Windows 10 64bit: Pro, Enterprise o Education (1607 Anniversary Update, Build 14393 o Superior)
    - Docker Desktop 
    - Powershell versión 7.x
    - Net 6 o superior
    - Visual Studio Code
    - Firefox

## CONSIDERACIONES INICIALES
  * Clonar el repositorio mediante git para tener los recursos necesarios

## DESARROLLO
1. Iniciar la aplicación Powershell o Windows Terminal en modo administrador 
2. En el Terminal, ejecutar el siguiente comando para crear una nueva solución
```
dotnet new mstest -n UPTSiteTests
```
3. En el Terminal, acceder a la solución creada y ejecutar el siguiente comando para adicionar los paquetes necesarios al proyecto.
```
cd UPTSiteTests
dotnet add package Microsoft.Playwright.MSTest
```
4. Iniciar Visual Studio Code (VS Code) abriendo el folder de la solución como proyecto. En el proyecto UPTSiteTests, si existiese un archivo UnitTest1.cs, proceder a eliminarlo.

6. En VS Code, en el proyecto UPTSiteTests proceder a crear el archivo UPTSiteTest.cs e introducir el siguiente código:
```C#
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.RegularExpressions;

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
        await Page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Textbox).ClickAsync();
        await Page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Textbox).FillAsync(studentSearch);
        await Page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Button, new() { Name = "Buscar" }).ClickAsync();
        await Page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Link, new() { Name = "CICLO - VII", Exact = true }).ClickAsync();

        // Assert
        await Expect(Page.Locator("iframe").ContentFrame.GetByRole(AriaRole.Table)).ToContainTextAsync(studentName);
    } 
}
```
7. En el Terminal, ejecutar los siguientes comandos para instalar los drivers de navegadores web necesarios para la ejecución de las pruebas:
```Bash
pwsh bin/Debug/net8.0/playwright.ps1 install
dotnet test --collect:"XPlat Code Coverage"
```
8. El paso anterior debe producir un resultado satisfactorio como el siguiente. 
```Bash
Correctas! - Con error:     0, Superado:     3, Omitido:     0, Total:     3, Duración: 12 s - UPTSiteTests.dll (net8.0)
```
9. En el terminal, proceder a ejecutar los siguientes comandos para visualizar los diferentes test en cada uno de los navegadores.
```Bash
$env:HEADED="1"
dotnet test -- Playwright.BrowserName=chromium
dotnet test -- Playwright.BrowserName=webkit
dotnet test -- Playwright.BrowserName=firefox
```
10. En el Terminal, proceder a revisar las trazas generadas por las diferentes pruebas, ejecutar el siguiente comando:
```Bash
pwsh bin/Debug/net8.0/playwright.ps1 show-trace .\bin\Debug\net8.0\playwright-traces\UPTSiteTests.UPTSiteTest.GetSchoolDirectorName.zip
```

![image](https://github.com/user-attachments/assets/75a15bf9-aa58-4e4f-bc8c-fafdeddb2d98)

11. Finalmente proceder a verificar la cobertura, dentro del proyecto Primes.Tests se dede haber generado una carpeta o directorio TestResults, en el cual posiblemente exista otra subpcarpeta o subdirectorio conteniendo un archivo con nombre `coverage.cobertura.xml`, si existe ese archivo proceder a ejecutar los siguientes comandos desde la linea de comandos abierta anteriomente, de los contrario revisar el paso 8:
```
dotnet tool install -g dotnet-reportgenerator-globaltool
ReportGenerator "-reports:./*/*/*/coverage.cobertura.xml" "-targetdir:Cobertura" -reporttypes:HTML
```
13. El comando anterior primero proceda instalar una herramienta llamada ReportGenerator (https://reportgenerator.io/) la cual mediante la segunda parte del comando permitira generar un reporte en formato HTML con la cobertura obtenida de la ejecución de las pruebas. Este reporte debe localizarse dentro de una carpeta llamada Cobertura y puede acceder a el abriendo con un navegador de internet el archivo index.htm.

---
## Actividades Encargadas

### ✅ Actividad 1: Adicionar al menos 2 escenarios de prueba más
**Estado:** COMPLETADA

Se agregaron 2 pruebas adicionales en `UPTSiteTest.cs`:
- `CheckAdmissionsPageContainsAdmissionsText()` - Verifica navegación a Pre-Grado
- `CheckFooterContainsContact()` - Verifica información de contacto en el footer

Total de pruebas: **5** (3 originales + 2 nuevas)

### ✅ Actividad 2: Automatización para CI y publicación de cobertura
**Estado:** COMPLETADA

Archivo: `.github/workflows/publish_cov_report.yml`

Funcionalidades implementadas:
- ✅ Compilación y ejecución de pruebas con Playwright
- ✅ Generación de reporte de cobertura (ReportGenerator)
- ✅ Publicación del reporte HTML en GitHub Pages
- ✅ Upload de videos de pruebas como artifacts
- ✅ Upload de trazas de Playwright como artifacts

**⚠️ NOTA IMPORTANTE:** El workflow requiere configurar el secreto `GH_PAT` para desplegar en GitHub Pages en repositorios de organizaciones. Ver `SOLUCION_ERROR_CI.md` para instrucciones detalladas.

### ✅ Actividad 3: Automatización de Release y NuGet
**Estado:** COMPLETADA

Archivo: `.github/workflows/release.yml`

Funcionalidades implementadas:
- ✅ Generación de paquete NuGet con código de matrícula como versión
- ✅ Publicación del paquete en GitHub Packages
- ✅ Creación automática de GitHub Release
- ✅ Ejecución manual vía workflow_dispatch o por tags

---
## Evidencias entregadas

### Archivos del proyecto
- `UPTSiteTests/UPTSiteTests.csproj` — Proyecto de pruebas MSTest con Playwright (net8.0)
- `UPTSiteTests/UPTSiteTest.cs` — 5 casos de prueba completos
- `UPTSiteTests/PlaywrightInstaller.cs` — Configuración de instalación de Playwright

### Workflows de CI/CD
- `.github/workflows/publish_cov_report.yml` — Pipeline de pruebas y publicación de cobertura
- `.github/workflows/release.yml` — Pipeline de empaquetado y release
- `.github/workflows/classroom.yml` — No modificado (como se solicitó)

### Documentación
- `EVIDENCIAS.md` — Instrucciones completas de ejecución local y evidencias
- `SOLUCION_ERROR_CI.md` — Guía detallada para resolver el error de autenticación en GitHub Pages
- `README.md` — Este archivo (actualizado)

### Estructura de directorios generados
```
UPTSiteTests/
├── videos/                    # Videos de ejecución de pruebas
├── playwright-traces/         # Trazas para debugging
└── TestResults/              # Reportes de cobertura XML
Cobertura/                    # Reporte HTML de cobertura (generado localmente)
```

---
## 🚨 Resolver Error de CI/CD

El workflow de GitHub Pages está fallando debido a permisos de autenticación. 

**Solución rápida:** Consulta el archivo `SOLUCION_ERROR_CI.md` para instrucciones paso a paso sobre cómo:
1. Crear un Personal Access Token (GH_PAT)
2. Agregarlo como secreto al repositorio
3. Re-ejecutar el workflow exitosamente

---
## 📊 Resultados esperados

Después de configurar GH_PAT y ejecutar los workflows:

- **GitHub Pages:** https://upt-faing-epis.github.io/lab-2025-ii-si784-u2-06-cs-diegocastillo12/
- **NuGet Package:** Disponible en GitHub Packages del repositorio
- **Release:** Creado automáticamente con el tag de versión

---
## 🎯 Resumen de cumplimiento

| Actividad | Estado | Detalles |
|-----------|--------|----------|
| 1. Dos pruebas adicionales | ✅ COMPLETO | 5 pruebas totales funcionando |
| 2. CI con cobertura y Pages | ✅ COMPLETO | Requiere configurar GH_PAT |
| 3. Release y NuGet | ✅ COMPLETO | Workflow funcional con workflow_dispatch |

**Todas las actividades solicitadas han sido completadas.**
